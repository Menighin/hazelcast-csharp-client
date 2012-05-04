using System;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using Hazelcast.Client.Impl;
using Hazelcast.Core;
using System.Threading;
using Hazelcast.Security;
using System.Net;
namespace Hazelcast.Client
{
	public class HazelcastClient: HazelcastInstance
	{
		private readonly OutThread outThread;		
		private readonly InThread inThread;
		private readonly ListenerManager listenerManager;
		private readonly ClusterClientProxy clusterClientProxy;
		private readonly LifecycleServiceClientImpl lifecycleService;
		private readonly ConnectionManager connectionManager;
		private readonly long id;
		private readonly ClientConfig config;
		private static long clientIdCounter = 0;
		
		//private readonly PartitionClientProxy partitionClientProxy;
		ConcurrentDictionary<long, Call> calls = new ConcurrentDictionary<long, Call>();
		Dictionary<Object, Object> proxies = new Dictionary<Object, Object>();
		
		private HazelcastClient(ClientConfig config)
		{			
			this.config = config;			
			if(config.TypeConverter!=null)
				TypeRegistry.setTypeConverter(config.TypeConverter);
			
			if(config.Credentials == null){
				UsernamePasswordCredentials cr = new UsernamePasswordCredentials();
				cr.setUsername(config.GroupConfig.Name);
				cr.setPassword(config.GroupConfig.Password);
				config.Credentials = cr;
			}
			
			if(config.AddressList.Count == 0){
				config.addAddress("localhost");
			}
			
			lifecycleService = new LifecycleServiceClientImpl(this);
			connectionManager = new ConnectionManager(this, config, lifecycleService);
			connectionManager.setBinder(new DefaultClientBinder(this));	
			this.outThread = new OutThread(connectionManager, calls);
			this.inThread = new InThread(connectionManager, calls, listenerManager);
			this.listenerManager = new ListenerManager(this);

			
			try {
	            Connection c = connectionManager.getInitConnection();
	            if (c == null) {
	                connectionManager.shutdown();
	                throw new Exception("No member available to connect.");
	            }
	        } catch (Exception e) {
	            connectionManager.shutdown();
				Console.Write(e.StackTrace);
	            throw new Exception("Unable to connect to cluster: " + e.Message);
	        }
			
			id = incrementId();
			String prefix = "hz.client." + this.id + ".";
			this.outThread.start(prefix);
			this.inThread.start(prefix);
			this.listenerManager.start(prefix);
			
			
			clusterClientProxy = new ClusterClientProxy(outThread, listenerManager, this);
			lifecycleService = new LifecycleServiceClientImpl(this);
    		//partitionClientProxy = new PartitionClientProxy(this);
			
		}
		
		public OutThread OutThread {
			get{ return outThread;}	
		}
		
		public InThread InThread {
			get{ return inThread;}	
		}
		
		public ClientConfig ClientConfig{
			get{ return config;}
		}
		
		public ListenerManager ListenerManager {
			get{ return listenerManager;}
		}
		
		public static HazelcastClient newHazelcastClient(ClientConfig config){
			
			UsernamePasswordCredentials credentials = new UsernamePasswordCredentials();
			credentials.setUsername(config.GroupConfig.Name);
			credentials.setPassword(config.GroupConfig.Password);
			config.Credentials = credentials;
			
			return new HazelcastClient(config);
		}

		private static Address parse(String address) {
	        String[] separated = address.Split(':');
	        int port = (separated.Length > 1) ? int.Parse(separated[1]) : 5701;
	        IPEndPoint iPEndPoint = new IPEndPoint(Dns.GetHostEntry(separated[0]).AddressList[0], port);
			return new Address(iPEndPoint);
	    }
		
		public String getName(){
			return null;	
		}
		
		public ICluster getCluster(){
			return clusterClientProxy;
		}
		
		public IAtomicNumber getAtomicNumber(String name) {
			return (IAtomicNumber)getClientProxy(Prefix.ATOMIC_NUMBER + name, () => new AtomicNumberClientProxy(outThread, Prefix.ATOMIC_NUMBER +name, listenerManager, this));
	    }
	
	    /*public ICountDownLatch getCountDownLatch(String name) {
	        return (ICountDownLatch) getClientProxy(Prefix.COUNT_DOWN_LATCH + name);
	    }
	
	    public ISemaphore getSemaphore(String name) {
	        return (ISemaphore) getClientProxy(Prefix.SEMAPHORE + name);
	    }*/
		
		public IMap<K,V> getMap<K,V>(String name){
			return (IMap<K,V>)getClientProxy(Prefix.MAP + name, ()=> new MapClientProxy<K, V>(outThread, Prefix.MAP + name, listenerManager, this));
		}

		public IMultiMap<K,V> getMultiMap<K,V>(String name){
			return (IMultiMap<K,V>)getClientProxy(Prefix.MULTIMAP + name, ()=> new MultiMapClientProxy<K, V>(outThread, Prefix.MULTIMAP + name, listenerManager, this));
		}
		
		public IQueue<E> getQueue<E>(String name){
			return (QueueClientProxy<E>)getClientProxy(Prefix.QUEUE + name, () => new QueueClientProxy<E>(outThread, Prefix.QUEUE +name, listenerManager, this));
		}
		
		public ITopic<E> getTopic<E>(String name){
			return (ITopic<E>)getClientProxy(Prefix.TOPIC + name, () => new TopicClientProxy<E>(outThread, Prefix.TOPIC +name, listenerManager, this));
		}
		
		public Hazelcast.Core.ISet<E> getSet<E>(String name){
			return (Hazelcast.Core.ISet<E>)getClientProxy(Prefix.SET + name, () => new SetClientProxy<E>(outThread, Prefix.SET +name, listenerManager, this));
		}
		
		public Hazelcast.Core.IList<E> getList<E>(String name){
			return (Hazelcast.Core.IList<E>)getClientProxy(Prefix.AS_LIST + name, () => new ListClientProxy<E>(outThread, Prefix.AS_LIST +name, listenerManager, this));
		}
		
		public IdGenerator getIdGenerator(String name){
			return (IdGenerator)getClientProxy(Prefix.IDGEN + name, () => new IdGeneratorClientProxy(outThread, Prefix.IDGEN +name, listenerManager, this));
		}
		
		public ILock getLock(Object obj) {
        	return new LockClientProxy(outThread, obj, this);
    	}
		
		public Hazelcast.Core.Transaction getTransaction() {
        	ClientThreadContext trc = ClientThreadContext.get();
        	TransactionClientProxy proxy = (TransactionClientProxy) trc.getTransaction(outThread, this);
        	return proxy;
   	 	}
		
		public System.Collections.Generic.ICollection<Instance> getInstances() {
    	    return clusterClientProxy.getInstances();
    	}
		
		public void addInstanceListener(InstanceListener instanceListener) {
	        clusterClientProxy.addInstanceListener(instanceListener);
	    }
		
		 public void removeInstanceListener(InstanceListener instanceListener) {
	        clusterClientProxy.removeInstanceListener(instanceListener);
	    }
		
		public LifecycleService getLifecycleService() {
	        return lifecycleService;
		}
		
		
		public Object getClientProxy(Object o){
			Object proxy = null;
			if(proxies.TryGetValue(o, out proxy)){
				return proxy;
			}
			
			if (o is String) {
                String name = (String) o;
				if (name.StartsWith(Prefix.MAP)) {
                    proxy = getMap<object, object>(name.Substring(Prefix.MAP.Length)); 
                } else if (name.StartsWith(Prefix.AS_LIST)) {
                    proxy = getList<object>(name.Substring(Prefix.AS_LIST.Length));
                } else if (name.StartsWith(Prefix.SET)) {
                    proxy = getSet<object>(name.Substring(Prefix.SET.Length));
                } else if (name.StartsWith(Prefix.QUEUE)) {
                    proxy = getQueue<object>(name.Substring(Prefix.QUEUE.Length));
                } else if (name.StartsWith(Prefix.TOPIC)) {
                    proxy = getTopic<object>(name.Substring(Prefix.TOPIC.Length));
                } else if (name.StartsWith(Prefix.ATOMIC_NUMBER)) {
                    proxy = getAtomicNumber(name.Substring(Prefix.ATOMIC_NUMBER.Length));
                } else if (name.StartsWith(Prefix.COUNT_DOWN_LATCH)) {
                    //proxy = get(name.Substring(Prefix.ATOMIC_NUMBER));
                } else if (name.StartsWith(Prefix.IDGEN)) {
                    proxy = getIdGenerator(name.Substring(Prefix.IDGEN.Length));
                } else if (name.StartsWith(Prefix.MULTIMAP)) {
                    proxy = getMultiMap<object, object>(name.Substring(Prefix.MULTIMAP.Length));
                } else if (name.StartsWith(Prefix.SEMAPHORE)) {
                    //proxy = new SemaphoreClientProxy(this, name);
                } else {
                    proxy = getLock(name);
                }
            } else {
                proxy = getLock(o);
            }
			return proxy;
		}
		
		
		
		
		private Object getClientProxy(Object o, Func<Object> func)
		{
			Object proxy=null;
        	if (!proxies.TryGetValue(o,out proxy)) {
				lock (proxies) {
		            if (!proxies.TryGetValue(o,out proxy)) {
		            	proxy = func.Invoke();
		               	proxies.Add(o, proxy);
		            }
	        	}
        	}
        	return proxies[o];
		}
		
		public void destroy(String name){
			lock(proxies){
				proxies.Remove(name);
			}
		}
		
		public void doShutdown() {
        
	        //connectionManager.shutdown();
	        outThread.shutdown();
	        inThread.shutdown();
	        listenerManager.shutdown();
	        //ClientThreadContext.shutdown();
	        //active = false;
	    }
		
		private static long incrementId ()
		{
			long initialValue, computedValue;
			do {
				initialValue = clientIdCounter;
				computedValue = initialValue + 1;
				
			} while (initialValue != Interlocked.CompareExchange (ref clientIdCounter, computedValue, initialValue));
			return computedValue;
		}
		
		
	}
}
