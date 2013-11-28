using System;
using System.Collections.Generic;
using System.Text;
using Hazelcast.Config;
using Hazelcast.Net.Ext;


namespace Hazelcast.Config
{
	public class SocketInterceptorConfig
	{
		private bool enabled = false;

		private string className = null;

		private object implementation = null;

        private Dictionary<string, string> properties = new Dictionary<string, string>();

		/// <summary>
		/// Returns the name of the
		/// <see cref="Hazelcast.IO.SocketInterceptor">Hazelcast.IO.SocketInterceptor</see>
		/// implementation class
		/// </summary>
		/// <returns>name of the class</returns>
		public virtual string GetClassName()
		{
			return className;
		}

		/// <summary>
		/// Sets the name for the
		/// <see cref="Hazelcast.IO.SocketInterceptor">Hazelcast.IO.SocketInterceptor</see>
		/// implementation class
		/// </summary>
		/// <param name="className">
		/// the name of the
		/// <see cref="Hazelcast.IO.SocketInterceptor">Hazelcast.IO.SocketInterceptor</see>
		/// implementation class to set
		/// </param>
		/// <returns>this SocketInterceptorConfig instance</returns>
		public virtual SocketInterceptorConfig SetClassName(string className)
		{
			this.className = className;
			return this;
		}

		/// <summary>
		/// Sets the
		/// <see cref="Hazelcast.IO.SocketInterceptor">Hazelcast.IO.SocketInterceptor</see>
		/// implementation object
		/// </summary>
		/// <param name="implementation">implementation object</param>
		/// <returns>this SocketInterceptorConfig instance</returns>
		public virtual SocketInterceptorConfig SetImplementation(object implementation)
		{
			this.implementation = implementation;
			return this;
		}

		/// <summary>
		/// Returns the
		/// <see cref="Hazelcast.IO.SocketInterceptor">Hazelcast.IO.SocketInterceptor</see>
		/// implementation object
		/// </summary>
		/// <returns>SocketInterceptor implementation object</returns>
		public virtual object GetImplementation()
		{
			return implementation;
		}

		/// <summary>Returns if this configuration is enabled</summary>
		/// <returns>true if enabled, false otherwise</returns>
		public virtual bool IsEnabled()
		{
			return enabled;
		}

		/// <summary>Enables and disables this configuration</summary>
		/// <param name="enabled"></param>
		public virtual SocketInterceptorConfig SetEnabled(bool enabled)
		{
			this.enabled = enabled;
			return this;
		}

		/// <summary>Sets a property.</summary>
		/// <remarks>Sets a property.</remarks>
		/// <param name="name">the name of the property to set.</param>
		/// <param name="value">the value of the property to set</param>
		/// <returns>the updated SocketInterceptorConfig</returns>
		/// <exception cref="System.ArgumentNullException">if name or value is null.</exception>
		public virtual SocketInterceptorConfig SetProperty(string name, string value)
		{
			properties.Add(name, value);
			return this;
		}

		/// <summary>Gets a property.</summary>
		/// <remarks>Gets a property.</remarks>
		/// <param name="name">the name of the property to get.</param>
		/// <returns>the value of the property, null if not found</returns>
		/// <exception cref="System.ArgumentNullException">if name is null.</exception>
		public virtual string GetProperty(string name)
		{
		    string value=null;
            properties.TryGetValue(name, out value);
		    return value;		
        }

		/// <summary>Gets all properties.</summary>
		/// <remarks>Gets all properties.</remarks>
		/// <returns>the properties.</returns>
		public virtual Dictionary<string,string> GetProperties()
		{
			return properties;
		}

		/// <summary>Sets the properties.</summary>
		/// <remarks>Sets the properties.</remarks>
		/// <param name="properties">the properties to set.</param>
		/// <returns>the updated SSLConfig.</returns>
		/// <exception cref="System.ArgumentException">if properties is null.</exception>
        public virtual SocketInterceptorConfig SetProperties(Dictionary<string, string> properties)
		{
			if (properties == null)
			{
				throw new ArgumentException("properties can't be null");
			}
			this.properties = properties;
			return this;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("SocketInterceptorConfig");
			sb.Append("{className='").Append(className).Append('\'');
			sb.Append(", enabled=").Append(enabled);
			sb.Append(", implementation=").Append(implementation);
			sb.Append(", properties=").Append(properties);
			sb.Append('}');
			return sb.ToString();
		}
	}
}