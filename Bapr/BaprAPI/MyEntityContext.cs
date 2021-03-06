﻿ 

// -----------------------------------------------------------------------
// <autogenerated>
//    This code was generated from a template.
//
//    Changes to this file may cause incorrect behaviour and will be lost
//    if the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using BrightstarDB.Client;
using BrightstarDB.EntityFramework;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web;

namespace BaprAPI 
{
    public partial class MyEntityContext : BrightstarEntityContext {
    	
    	static MyEntityContext() 
    	{
            InitializeEntityMappingStore();
        }
        
        /// <summary>
        /// Initialize the internal cache of entity attribute information.
        /// </summary>
        /// <remarks>
        /// This method is normally invoked from the static constructor for the generated context class.
        /// It is provided as a public static method to enable the use of the cached entity attribute 
        /// information without the need to construct a context (typically in test code). 
        /// In normal application code you should never need to explicitly call this method.
        /// </remarks>
        public static void InitializeEntityMappingStore()
        {
    		var provider = new ReflectionMappingProvider();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BaprAPI.Models.ILocation));
    		EntityMappingStore.Instance.SetImplMapping<BaprAPI.Models.ILocation, BaprAPI.Models.Location>();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BaprAPI.Models.ILocationAttribute));
    		EntityMappingStore.Instance.SetImplMapping<BaprAPI.Models.ILocationAttribute, BaprAPI.Models.LocationAttribute>();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BaprAPI.Models.IUser));
    		EntityMappingStore.Instance.SetImplMapping<BaprAPI.Models.IUser, BaprAPI.Models.User>();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BaprAPI.Models.IInterest));
    		EntityMappingStore.Instance.SetImplMapping<BaprAPI.Models.IInterest, BaprAPI.Models.Interest>();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BaprAPI.Models.IUserPreference));
    		EntityMappingStore.Instance.SetImplMapping<BaprAPI.Models.IUserPreference, BaprAPI.Models.UserPreference>();
    	}
    	
    	/// <summary>
    	/// Initialize a new entity context using the specified BrightstarDB
    	/// Data Object Store connection
    	/// </summary>
    	/// <param name="dataObjectStore">The connection to the BrightstarDB Data Object Store that will provide the entity objects</param>
    	public MyEntityContext(IDataObjectStore dataObjectStore) : base(dataObjectStore)
    	{
    		InitializeContext();
    	}
    
    	/// <summary>
    	/// Initialize a new entity context using the specified Brightstar connection string
    	/// </summary>
    	/// <param name="connectionString">The connection to be used to connect to an existing BrightstarDB store</param>
    	/// <param name="enableOptimisticLocking">OPTIONAL: If set to true optmistic locking will be applied to all entity updates</param>
        /// <param name="updateGraphUri">OPTIONAL: The URI identifier of the graph to be updated with any new triples created by operations on the store. If
        /// not defined, the default graph in the store will be updated.</param>
        /// <param name="datasetGraphUris">OPTIONAL: The URI identifiers of the graphs that will be queried to retrieve entities and their properties.
        /// If not defined, all graphs in the store will be queried.</param>
        /// <param name="versionGraphUri">OPTIONAL: The URI identifier of the graph that contains version number statements for entities. 
        /// If not defined, the <paramref name="updateGraphUri"/> will be used.</param>
    	public MyEntityContext(
    	    string connectionString, 
    		bool? enableOptimisticLocking=null,
    		string updateGraphUri = null,
    		IEnumerable<string> datasetGraphUris = null,
    		string versionGraphUri = null
        ) : base(connectionString, enableOptimisticLocking, updateGraphUri, datasetGraphUris, versionGraphUri)
    	{
    		InitializeContext();
    	}
    
    	/// <summary>
    	/// Initialize a new entity context using the specified Brightstar
    	/// connection string retrieved from the configuration.
    	/// </summary>
    	public MyEntityContext() : base()
    	{
    		InitializeContext();
    	}
    	
    	/// <summary>
    	/// Initialize a new entity context using the specified Brightstar
    	/// connection string retrieved from the configuration and the
    	//  specified target graphs
    	/// </summary>
        /// <param name="updateGraphUri">The URI identifier of the graph to be updated with any new triples created by operations on the store. If
        /// set to null, the default graph in the store will be updated.</param>
        /// <param name="datasetGraphUris">The URI identifiers of the graphs that will be queried to retrieve entities and their properties.
        /// If set to null, all graphs in the store will be queried.</param>
        /// <param name="versionGraphUri">The URI identifier of the graph that contains version number statements for entities. 
        /// If set to null, the value of <paramref name="updateGraphUri"/> will be used.</param>
    	public MyEntityContext(
    		string updateGraphUri,
    		IEnumerable<string> datasetGraphUris,
    		string versionGraphUri
    	) : base(updateGraphUri:updateGraphUri, datasetGraphUris:datasetGraphUris, versionGraphUri:versionGraphUri)
    	{
    		InitializeContext();
    	}
    	
    	private void InitializeContext() 
    	{
    		Locations = 	new BrightstarEntitySet<BaprAPI.Models.ILocation>(this);
    		LocationAttributes = 	new BrightstarEntitySet<BaprAPI.Models.ILocationAttribute>(this);
    		Users = 	new BrightstarEntitySet<BaprAPI.Models.IUser>(this);
    		Interests = 	new BrightstarEntitySet<BaprAPI.Models.IInterest>(this);
    		UserPreferences = 	new BrightstarEntitySet<BaprAPI.Models.IUserPreference>(this);
    	}
    	
    	public IEntitySet<BaprAPI.Models.ILocation> Locations
    	{
    		get; private set;
    	}
    	
    	public IEntitySet<BaprAPI.Models.ILocationAttribute> LocationAttributes
    	{
    		get; private set;
    	}
    	
    	public IEntitySet<BaprAPI.Models.IUser> Users
    	{
    		get; private set;
    	}
    	
    	public IEntitySet<BaprAPI.Models.IInterest> Interests
    	{
    		get; private set;
    	}
    	
    	public IEntitySet<BaprAPI.Models.IUserPreference> UserPreferences
    	{
    		get; private set;
    	}
    	
        public IEntitySet<T> EntitySet<T>() where T : class {
            var itemType = typeof(T);
            if (typeof(T).Equals(typeof(BaprAPI.Models.ILocation))) {
                return (IEntitySet<T>)this.Locations;
            }
            if (typeof(T).Equals(typeof(BaprAPI.Models.ILocationAttribute))) {
                return (IEntitySet<T>)this.LocationAttributes;
            }
            if (typeof(T).Equals(typeof(BaprAPI.Models.IUser))) {
                return (IEntitySet<T>)this.Users;
            }
            if (typeof(T).Equals(typeof(BaprAPI.Models.IInterest))) {
                return (IEntitySet<T>)this.Interests;
            }
            if (typeof(T).Equals(typeof(BaprAPI.Models.IUserPreference))) {
                return (IEntitySet<T>)this.UserPreferences;
            }
            throw new InvalidOperationException(typeof(T).FullName + " is not a recognized entity interface type.");
        }
    
        } // end class MyEntityContext
        
}
namespace BaprAPI.Models 
{
    	
    	[Newtonsoft.Json.JsonConverterAttribute(typeof(LocationConverter))]
    public partial class Location : BrightstarEntityObject, ILocation 
    {
    	public Location(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public Location(BrightstarEntityContext context) : base(context, typeof(Location)) { }
    	public Location() : base() { }
    	#region Implementation of BaprAPI.Models.ILocation
    
    	public System.Double latitude
    	{
            		get { return GetRelatedProperty<System.Double>("latitude"); }
            		set { SetRelatedProperty("latitude", value); }
    	}
    
    	public System.Double longitude
    	{
            		get { return GetRelatedProperty<System.Double>("longitude"); }
            		set { SetRelatedProperty("longitude", value); }
    	}
    
    	public System.String name
    	{
            		get { return GetRelatedProperty<System.String>("name"); }
            		set { SetRelatedProperty("name", value); }
    	}
    
    	public System.Boolean IsVisited
    	{
            		get { return GetRelatedProperty<System.Boolean>("IsVisited"); }
            		set { SetRelatedProperty("IsVisited", value); }
    	}
    
    	public System.Boolean IsFavorite
    	{
            		get { return GetRelatedProperty<System.Boolean>("IsFavorite"); }
            		set { SetRelatedProperty("IsFavorite", value); }
    	}
    	public System.Collections.Generic.ICollection<BaprAPI.Models.ILocationAttribute> attributes
    	{
    		get { return GetRelatedObjects<BaprAPI.Models.ILocationAttribute>("attributes"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("attributes", value); }
    								}
    	#endregion
    }
}
namespace BaprAPI.Models 
{
    
    public partial class LocationAttribute : BrightstarEntityObject, ILocationAttribute 
    {
    	public LocationAttribute(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public LocationAttribute(BrightstarEntityContext context) : base(context, typeof(LocationAttribute)) { }
    	public LocationAttribute() : base() { }
    	#region Implementation of BaprAPI.Models.ILocationAttribute
    
    	public System.String Name
    	{
            		get { return GetRelatedProperty<System.String>("Name"); }
            		set { SetRelatedProperty("Name", value); }
    	}
    
    	public System.String Value
    	{
            		get { return GetRelatedProperty<System.String>("Value"); }
            		set { SetRelatedProperty("Value", value); }
    	}
    
    	public System.String Type
    	{
            		get { return GetRelatedProperty<System.String>("Type"); }
            		set { SetRelatedProperty("Type", value); }
    	}
    	#endregion
    }
}
namespace BaprAPI.Models 
{
    
    public partial class User : BrightstarEntityObject, IUser 
    {
    	public User(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public User(BrightstarEntityContext context) : base(context, typeof(User)) { }
    	public User() : base() { }
    	#region Implementation of BaprAPI.Models.IUser
    
    	public System.String Email
    	{
            		get { return GetRelatedProperty<System.String>("Email"); }
            		set { SetRelatedProperty("Email", value); }
    	}
    
    	public System.String Password
    	{
            		get { return GetRelatedProperty<System.String>("Password"); }
            		set { SetRelatedProperty("Password", value); }
    	}
    
    	public BaprAPI.Models.IUserPreference UserPreference
    	{
            get { return GetRelatedObject<BaprAPI.Models.IUserPreference>("UserPreference"); }
            set { SetRelatedObject<BaprAPI.Models.IUserPreference>("UserPreference", value); }
    	}
    	public System.Collections.Generic.ICollection<BaprAPI.Models.ILocation> MarkedLocations
    	{
    		get { return GetRelatedObjects<BaprAPI.Models.ILocation>("MarkedLocations"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("MarkedLocations", value); }
    								}
    	#endregion
    }
}
namespace BaprAPI.Models 
{
    
    public partial class Interest : BrightstarEntityObject, IInterest 
    {
    	public Interest(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public Interest(BrightstarEntityContext context) : base(context, typeof(Interest)) { }
    	public Interest() : base() { }
    	#region Implementation of BaprAPI.Models.IInterest
    
    	public System.String Name
    	{
            		get { return GetRelatedProperty<System.String>("Name"); }
            		set { SetRelatedProperty("Name", value); }
    	}
    
    	public System.Boolean Checked
    	{
            		get { return GetRelatedProperty<System.Boolean>("Checked"); }
            		set { SetRelatedProperty("Checked", value); }
    	}
    	#endregion
    }
}
namespace BaprAPI.Models 
{
    
    public partial class UserPreference : BrightstarEntityObject, IUserPreference 
    {
    	public UserPreference(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public UserPreference(BrightstarEntityContext context) : base(context, typeof(UserPreference)) { }
    	public UserPreference() : base() { }
    	#region Implementation of BaprAPI.Models.IUserPreference
    	public System.Collections.Generic.ICollection<BaprAPI.Models.IInterest> Interests
    	{
    		get { return GetRelatedObjects<BaprAPI.Models.IInterest>("Interests"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("Interests", value); }
    								}
    	public System.Collections.Generic.ICollection<BaprAPI.Models.IInterest> Cuisine
    	{
    		get { return GetRelatedObjects<BaprAPI.Models.IInterest>("Cuisine"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("Cuisine", value); }
    								}
    
    	public System.Boolean NeedWheelchair
    	{
            		get { return GetRelatedProperty<System.Boolean>("NeedWheelchair"); }
            		set { SetRelatedProperty("NeedWheelchair", value); }
    	}
    	#endregion
    }
}
