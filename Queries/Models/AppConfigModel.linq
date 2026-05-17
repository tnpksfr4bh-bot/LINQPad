<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

// --- Test ---
var bundle = new Models.App.Bundle { aid = "a-001", bundle_id = "com.example.app", status = "live" };
bundle.Dump();
var lt = new Models.App.LocaleText { key = "hello", ko_kr = "안녕", en_us = "Hello" };
Console.WriteLine(lt.GetText("ko_kr")); // 안녕

// ---

namespace Models.App
{
	[Table("game_db_config")]
	public class GameDbConfig
	{
		[Key][Column("id")]                    public int    id                  { get; set; }
		[Column("aid")]                        public string aid                 { get; set; }
		[Column("server_ip")]                  public string server_ip           { get; set; }
		[Column("user_id")]                    public string user_id             { get; set; }
		[Column("password")]                   public string password            { get; set; }
		[Column("dbname")]                     public string dbname              { get; set; }
		[Column("cluster_server_ip")]          public string cluster_server_ip   { get; set; }
		[Column("cluster_user_id")]            public string cluster_user_id     { get; set; }
		[Column("cluster_password")]           public string cluster_password    { get; set; }
		[Column("cluster_dbname")]             public string cluster_dbname      { get; set; }

		public string GetWriteConnectionString(uint port = 3306, uint min = 1, uint max = 10)
			=> $"Server={server_ip};Port={port};user id={user_id};password={password};Database={dbname};minimumPoolSize={min};maximumpoolsize={max};SSL Mode=None;";

		public string GetReadConnectionString(uint port = 3306, uint min = 1, uint max = 10)
			=> $"Server={cluster_server_ip};Port={port};user id={cluster_user_id};password={cluster_password};Database={cluster_dbname};minimumPoolSize={min};maximumpoolsize={max};SSL Mode=None;";
	}

	[Table("game_table_config")]
	public class GameTableConfig
	{
		[Key][Column("id")]         public int    id           { get; set; }
		[Column("aid")]             public string aid          { get; set; }
		[Column("data_table")]      public string data_table   { get; set; }
		[Column("table_number")]    public int    table_number { get; set; }

		public string GetFixedDataTableName() => $"{data_table}_{table_number}";
	}

	[Table("info")]
	public class Info
	{
		[Key][Column("aid")]          public string aid             { get; set; }
		[Column("bundle_id")]         public string bundle_id       { get; set; }
		[Column("apple_id")]          public string apple_id        { get; set; }
		[Column("app_name")]          public string app_name        { get; set; }
		[Column("code_name")]         public string code_name       { get; set; }
		[Column("rsa_private_key")]   public string rsa_private_key { get; set; }
		[Column("rsa_public_key")]    public string rsa_public_key  { get; set; }
		[Column("status")]            public string status          { get; set; }
	}

	[Table("bundle")]
	public class Bundle
	{
		[Key][Column("aid")]          public string aid             { get; set; }
		[Column("bundle_id")]         public string bundle_id       { get; set; }
		[Column("apple_id")]          public string apple_id        { get; set; }
		[Column("app_name")]          public string app_name        { get; set; }
		[Column("rsa_private_key")]   public string rsa_private_key { get; set; }
		[Column("rsa_public_key")]    public string rsa_public_key  { get; set; }
		[Column("tdes_key")]          public string tdes_key        { get; set; }
		[Column("jwt_key")]           public string jwt_key         { get; set; }
		[Column("status")]            public string status          { get; set; }
		[Column("version")]           public string version         { get; set; }
	}

	[Table("apple_client")]
	public class AppleClient
	{
		[Key][Column("client_id")]    public string client_id    { get; set; }
		[Column("client_secret")]     public string client_secret { get; set; }
		[Column("key_id")]            public string key_id       { get; set; }
		[Column("team_id")]           public string team_id      { get; set; }
		[Column("bundle_id")]         public string bundle_id    { get; set; }
		[Column("aid")]               public string aid          { get; set; }
	}

	[Table("auth_client")]
	public class AuthClient
	{
		[Key][Column("id")]         public int    id            { get; set; }
		[Column("platform")]        public string platform      { get; set; }
		[Column("client_id")]       public string client_id     { get; set; }
		[Column("client_secret")]   public string client_secret { get; set; }
	}

	[Table("locale")]
	public class LocaleText
	{
		[Column("key")]   public string key   { get; set; }
		[Column("ko_kr")] public string ko_kr { get; set; }
		[Column("en_us")] public string en_us { get; set; }
		[Column("ja_jp")] public string ja_jp { get; set; }
		[Column("zh_tw")] public string zh_tw { get; set; }
		[Column("zh_cn")] public string zh_cn { get; set; }
		[Column("es_es")] public string es_es { get; set; }
		[Column("fr_fr")] public string fr_fr { get; set; }
		[Column("de_de")] public string de_de { get; set; }
		[Column("pt_pt")] public string pt_pt { get; set; }
		[Column("it_it")] public string it_it { get; set; }

		public string GetText(string locale)
		{
			if (locale == null) return en_us ?? key;
			locale = locale.Replace("-", "_");
			string v =
				Match("ko_kr", locale) ? ko_kr :
				Match("en_us", locale) ? en_us :
				Match("ja_jp", locale) ? ja_jp :
				Match("zh_tw", locale) ? zh_tw :
				Match("zh_cn", locale) ? zh_cn :
				Match("es_es", locale) ? es_es :
				Match("fr_fr", locale) ? fr_fr :
				Match("de_de", locale) ? de_de :
				Match("pt_pt", locale) ? pt_pt :
				Match("it_it", locale) ? it_it : null;
			return v ?? en_us ?? key;
		}

		private static bool Match(string col, string locale)
			=> col.StartsWith(locale, StringComparison.OrdinalIgnoreCase)
			|| col.EndsWith(locale, StringComparison.OrdinalIgnoreCase)
			|| col.Equals(locale, StringComparison.OrdinalIgnoreCase);
	}
}
