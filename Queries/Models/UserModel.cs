using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.User
{
	[Table("player")]
	public class Player
	{
		[Key][Column("pid")]           public string   pid           { get; set; }
		[Column("platform")]           public string   platform      { get; set; }
		[Column("uid")]                public string   uid           { get; set; }
		[Column("name")]               public string   name          { get; set; }
		[Column("email")]              public string   email         { get; set; }
		[Column("photo")]              public string   photo         { get; set; }
		[Column("register_time")]      public DateTime register_time { get; set; }
		[Column("update_time")]        public DateTime update_time   { get; set; }
	}

	[Table("auth_payload")]
	public class AuthPayload
	{
		[Column("id")]             public long     id            { get; set; }
		[Column("duid")]           public string   duid          { get; set; }
		[Column("platform")]       public string   platform      { get; set; }
		[Column("payload")]        public string   payload       { get; set; }
		[Column("update_time")]    public DateTime update_time   { get; set; }
		[Column("register_time")]  public DateTime register_time { get; set; }
	}

	[Table("player")]
	public class PlayerEntity
	{
		[Column("id")]             public long     id            { get; set; }
		[Column("sid")]            public string   sid           { get; set; }
		[Column("aid")]            public string   aid           { get; set; }
		[Column("pid")]            public string   pid           { get; set; }
		[Column("token")]          public string   token         { get; set; }
		[Column("user_id")]        public string   user_id       { get; set; }
		[Column("duid")]           public string   duid          { get; set; }
		[Column("device_id")]      public long     device_id     { get; set; }
		[Column("connection_id")]  public long     connection_id { get; set; }
		[Column("update_time")]    public DateTime update_time   { get; set; }
		[Column("register_time")]  public DateTime register_time { get; set; }
	}

	public class PlayerEntityWithDevice
	{
		public long     id            { get; set; }
		public string   sid           { get; set; }
		public string   aid           { get; set; }
		public string   pid           { get; set; }
		public string   user_id       { get; set; }
		public string   duid          { get; set; }
		public string   bundle_id     { get; set; }
		public string   version       { get; set; }
		public string   platform      { get; set; }
		public string   adid          { get; set; }
		public string   os            { get; set; }
		public string   ip            { get; set; }
		public string   country_iso   { get; set; }
		public string   operator_name { get; set; }
		public DateTime update_time   { get; set; }
		public DateTime register_time { get; set; }
	}

	[Table("string")]
	[Obsolete]
	public class StringTable
	{
		[Key][Column("key")]   public string key   { get; set; }
		[Column("ko_kr")]      public string ko_kr { get; set; }
		[Column("en_us")]      public string en_us { get; set; }
		[Column("ja_jp")]      public string ja_jp { get; set; }
		[Column("zh_cn")]      public string zh_cn { get; set; }
		[Column("zh_tw")]      public string zh_tw { get; set; }
	}
}
