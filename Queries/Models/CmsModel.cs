using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Cms
{
	[Table("api_history")]
	public class ApiHistory
	{
		[Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("id")]
		public ulong    id            { get; set; }
		[Column("action_id")]    public string    action_id    { get; set; }
		[Column("trace_id")]     public string    trace_id     { get; set; }
		[Column("method")]       public string    method       { get; set; }
		[Column("controller")]   public string    controller   { get; set; }
		[Column("action")]       public string    action       { get; set; }
		[Column("arguments")]    public string    arguments    { get; set; }
		[Column("register_time")] public DateTime register_time { get; set; }
		[Column("reason")]       public string    reason       { get; set; }
		[Column("result")]       public string?   result       { get; set; }
		[Column("update_time")]  public DateTime? update_time  { get; set; }
	}

	[Table("user_session")]
	public class UserSession
	{
		[Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("id")]
		public long   id     { get; set; }
		[Column("aid")]    public string aid    { get; set; }
		[Column("sid")]    public string sid    { get; set; }
		[Column("status")] public string status { get; set; }
	}

	[Table("auth")]
	public class Auth
	{
		[Key][Column("id")]               public string   id            { get; set; }
		[Column("email")]                 public string   email         { get; set; }
		[Column("name")]                  public string   name          { get; set; }
		[Column("level")]                 public int      level         { get; set; }
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("access_time")]
		public DateTime access_time   { get; set; }
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("register_time")]
		public DateTime register_time { get; set; }
	}

	[Table("permission")]
	public class Permission
	{
		[Key][Column("id")]  public int    id    { get; set; }
		[Column("aid")]      public string aid   { get; set; }
		[Column("uid")]      public string uid   { get; set; }
		[Column("level")]    public int    level { get; set; }
	}

	[Table("whitelist")]
	public class Whitelist
	{
		[Column("pid")]           public string   pid           { get; set; }
		[Column("register_time")] public DateTime register_time { get; set; }
	}

	[Table("client_info")]
	public class ClientInfo
	{
		[Key][Column("bundle_id")]    public string bundle_id     { get; set; }
		[Column("sdk_version")]       public string sdk_version   { get; set; }
		[Column("product_name")]      public string product_name  { get; set; }
		[Column("unity_version")]     public string unity_version { get; set; }
		[Column("rpc_version")]       public string rpc_version   { get; set; }
	}

	[Table("platform_settings")]
	public class PlatformSettings
	{
		[Key][Column("bundle_id")]                 public string bundle_id                  { get; set; }
		[Column("application_id")]                 public string application_id              { get; set; }
		[Column("google_service_info_json")]        public string google_service_info_json    { get; set; }
		[Column("google_service_info_plist")]       public string google_service_info_plist   { get; set; }
		[Column("google_app_id")]                   public string google_app_id               { get; set; }
		[Column("google_android_client_id")]        public string google_android_client_id    { get; set; }
		[Column("google_ios_client_id")]            public string google_ios_client_id        { get; set; }
		[Column("apple_service_id")]                public string apple_service_id            { get; set; }
		[Column("facebook_app_id")]                 public string facebook_app_id             { get; set; }
		[Column("facebook_client_token")]           public string facebook_client_token       { get; set; }
	}

	[Table("apple_store")]
	public class AppleStore
	{
		[Key][Column("bundle_id")]  public string  bundle_id   { get; set; }
		[Column("version")]         public string  version     { get; set; }
		[Column("rating")]          public decimal rating      { get; set; }
		[Column("track_id")]        public string  track_id    { get; set; }
		[Column("description")]     public string  description { get; set; }
	}

	[Table("google_store")]
	public class GoogleStore
	{
		[Key][Column("bundle_id")]  public string  bundle_id { get; set; }
		[Column("version")]         public string  version   { get; set; }
		[Column("rating")]          public decimal rating    { get; set; }
	}

	[Table("app_db")]
	public class AppDb
	{
		[Column("aid")]            public int    Aid           { get; set; }
		[Column("stat_name")]      public string StatName      { get; set; }
		[Column("log_name")]       public string LogName       { get; set; }
		[Column("log_server_url")] public string LogServerUrl  { get; set; }
		[Column("bg_color")]       public string BgColor       { get; set; }
	}
}
