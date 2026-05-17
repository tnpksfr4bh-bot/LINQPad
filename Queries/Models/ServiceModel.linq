<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
</Query>

// --- Test ---
var session = new Models.Service.PlayerSession { sid = "s-001", aid = "a-001", status = "" };
session.Dump();

// ---

namespace Models.Service
{
	[Table("connection")]
	public class Connection
	{
		[Key][Column("id")]        public long     id          { get; set; }
		[Column("duid")]           public string   duid        { get; set; }
		[Column("bundle_id")]      public string   bundle_id   { get; set; }
		[Column("version")]        public string   version     { get; set; }
		[Column("sid")]            public string   sid         { get; set; }
		[Column("aid")]            public string   aid         { get; set; }
		[Column("pid")]            public string   pid         { get; set; }
		[Column("login_time")]     public DateTime login_time  { get; set; }
		[Column("logout_time")]    public DateTime logout_time { get; set; }
	}

	[Table("device")]
	public class Device
	{
		[Key][Column("id")]          public long     id            { get; set; }
		[Column("duid")]             public string   duid          { get; set; }
		[Column("bundle_id")]        public string   bundle_id     { get; set; }
		[Column("version")]          public string   version       { get; set; }
		[Column("platform")]         public string   platform      { get; set; }
		[Column("adid")]             public string   adid          { get; set; }
		[Column("os")]               public string   os            { get; set; }
		[Column("ip")]               public string   ip            { get; set; }
		[Column("ipv6")]             public string   ipv6          { get; set; }
		[Column("country_iso")]      public string   country_iso   { get; set; }
		[Column("operator_name")]    public string   operator_name { get; set; }
		[Column("register_time")]    public DateTime register_time { get; set; }
	}

	[Table("player_0")]
	public class Player_0
	{
		[Key][Column("id")]          public long     id            { get; set; }
		[Column("sid")]              public string   sid           { get; set; }
		[Column("aid")]              public string   aid           { get; set; }
		[Column("pid")]              public string   pid           { get; set; }
		[Column("token")]            public string   token         { get; set; }
		[Column("user_id")]          public string   user_id       { get; set; }
		[Column("duid")]             public string   duid          { get; set; }
		[Column("name")]             public string   name          { get; set; }
		[Column("device_id")]        public long     device_id     { get; set; }
		[Column("connection_id")]    public long     connection_id { get; set; }
		[Column("update_time")]      public DateTime update_time   { get; set; }
		[Column("register_time")]    public DateTime register_time { get; set; }
	}

	[Table("playersession")]
	public class PlayerSession
	{
		[Key][Column("id")]             public long     id                { get; set; }
		[Column("expire_time")]         public DateTime expire_time       { get; set; }
		[Column("sid")]                 public string   sid               { get; set; }
		[Column("aid")]                 public string   aid               { get; set; }
		[Column("pid")]                 public string   pid               { get; set; }
		[Column("active_duid")]         public string   active_duid       { get; set; }
		[Column("active_token")]        public string   active_token      { get; set; }
		[Column("expired_duid")]        public string   expired_duid      { get; set; }
		[Column("expired_token")]       public string   expired_token     { get; set; }
		[Column("register_time")]       public DateTime register_time     { get; set; }
		[Column("access_time")]         public DateTime access_time       { get; set; }
		[Column("status")]              public string   status            { get; set; }
		[Column("reason")]              public string   reason            { get; set; }
		[Column("status_start_time")]   public DateTime status_start_time { get; set; }
		[Column("status_end_time")]     public DateTime status_end_time   { get; set; }
	}

	[Table("playerlink")]
	public class PlayerLink
	{
		[Key][Column("id")]          public long     id            { get; set; }
		[Column("lid")]              public string   lid           { get; set; }
		[Column("user_id")]          public string   user_id       { get; set; }
		[Column("sid")]              public string   sid           { get; set; }
		[Column("aid")]              public string   aid           { get; set; }
		[Column("pid")]              public string   pid           { get; set; }
		[Column("register_time")]    public DateTime register_time { get; set; }
		[Column("update_time")]      public DateTime update_time   { get; set; }
	}

	[Table("playerreferral")]
	public class PlayerReferral
	{
		[Key][Column("id")]          public long     id            { get; set; }
		[Column("referral_code")]    public string   referral_code { get; set; }
		[Column("sid")]              public string   sid           { get; set; }
		[Column("aid")]              public string   aid           { get; set; }
		[Column("pid")]              public string   pid           { get; set; }
		[Column("count")]            public int      count         { get; set; }
		[Column("register_time")]    public DateTime register_time { get; set; }
		[Column("update_time")]      public DateTime update_time   { get; set; }
	}

	[Table("coupon")]
	public class Coupon
	{
		[Key][Column("coupon_id")]   public int      coupon_id     { get; set; }
		[Column("code")]             public string   code          { get; set; }
		[Column("start_time")]       public DateTime start_time    { get; set; }
		[Column("end_time")]         public DateTime end_time      { get; set; }
		[Column("item_type")]        public string   item_type     { get; set; }
		[Column("item_id")]          public string   item_id       { get; set; }
		[Column("item_quantity")]    public int      item_quantity { get; set; }
		[Column("use_count")]        public int      use_count     { get; set; }
		[Column("limit_count")]      public int      limit_count   { get; set; }
	}

	[Table("coupon_0")]
	public class Coupon_0
	{
		[Key][Column("id")]          public long     id            { get; set; }
		[Column("coupon_id")]        public int      coupon_id     { get; set; }
		[Column("code")]             public string   code          { get; set; }
		[Column("aid")]              public string   aid           { get; set; }
		[Column("pid")]              public string   pid           { get; set; }
		[Column("sid")]              public string   sid           { get; set; }
		[Column("register_time")]    public DateTime register_time { get; set; }
	}

	[Table("mailbox")]
	public class Mailbox
	{
		[Key][Column("mailbox_id")]  public int      mailbox_id    { get; set; }
		[Column("sender")]           public string   sender        { get; set; }
		[Column("description")]      public string   description   { get; set; }
		[Column("start_time")]       public DateTime start_time    { get; set; }
		[Column("end_time")]         public DateTime end_time      { get; set; }
		[Column("item_type")]        public string   item_type     { get; set; }
		[Column("item_id")]          public string   item_id       { get; set; }
		[Column("item_quantity")]    public int      item_quantity { get; set; }
		[Column("is_secret")]        public bool     is_secret     { get; set; }
		[Column("country_iso")]      public string   country_iso   { get; set; }
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("newuser_time")]
		public DateTime? newuser_time { get; set; }
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("ab_target")]
		public string?   ab_target    { get; set; }
	}

	[Table("mailbox_0")]
	public class Mailbox_0
	{
		[Key][Column("id")]          public long     id            { get; set; }
		[Column("mailbox_id")]       public int      mailbox_id    { get; set; }
		[Column("aid")]              public string   aid           { get; set; }
		[Column("pid")]              public string   pid           { get; set; }
		[Column("sid")]              public string   sid           { get; set; }
		[Column("item_type")]        public string   item_type     { get; set; }
		[Column("item_id")]          public string   item_id       { get; set; }
		[Column("item_quantity")]    public int      item_quantity { get; set; }
		[Column("expired")]          public bool     expired       { get; set; }
		[Column("received")]         public bool     received      { get; set; }
		[Column("start_time")]       public DateTime start_time    { get; set; }
		[Column("end_time")]         public DateTime end_time      { get; set; }
		[Column("register_time")]    public DateTime register_time { get; set; }
	}

	[Table("mailboxtarget")]
	public class MailboxTarget
	{
		[Key][Column("id")]          public long   id         { get; set; }
		[Column("mailbox_id")]       public int    mailbox_id { get; set; }
		[Column("aid")]              public string aid        { get; set; }
		[Column("pid")]              public string pid        { get; set; }
		[Column("sid")]              public string sid        { get; set; }
	}

	[Table("notice")]
	public class Notice
	{
		[Key][Column("notice_id")]   public int      notice_id   { get; set; }
		[Column("country")]          public string   country     { get; set; }
		[Column("notice_type")]      public string   notice_type { get; set; }
		[Column("title")]            public string   title       { get; set; }
		[Column("text")]             public string   text        { get; set; }
		[Column("image_url")]        public string   image_url   { get; set; }
		[Column("action_type")]      public string   action_type { get; set; }
		[Column("action_url")]       public string   action_url  { get; set; }
		[Column("start_time")]       public DateTime start_time  { get; set; }
		[Column("end_time")]         public DateTime end_time    { get; set; }
		[Column("is_secret")]        public bool     is_secret   { get; set; }
		[Column("coupon_code")]      public string   coupon_code { get; set; }
		[Column("sort")]             public int      sort        { get; set; }
	}

	[Table("notice_0")]
	public class Notice_0
	{
		[Key][Column("id")]          public long     id            { get; set; }
		[Column("notice_id")]        public int      notice_id     { get; set; }
		[Column("aid")]              public string   aid           { get; set; }
		[Column("pid")]              public string   pid           { get; set; }
		[Column("sid")]              public string   sid           { get; set; }
		[Column("register_time")]    public DateTime register_time { get; set; }
	}

	[Table("noticetarget")]
	public class NoticeTarget
	{
		[Key][Column("id")]          public long   id        { get; set; }
		[Column("notice_id")]        public int    notice_id { get; set; }
		[Column("aid")]              public string aid       { get; set; }
		[Column("pid")]              public string pid       { get; set; }
		[Column("sid")]              public string sid       { get; set; }
	}

	[Table("referral_0")]
	public class Referral_0
	{
		[Key][Column("id")]          public long     id            { get; set; }
		[Column("referral_code")]    public string   referral_code { get; set; }
		[Column("sid")]              public string   sid           { get; set; }
		[Column("aid")]              public string   aid           { get; set; }
		[Column("pid")]              public string   pid           { get; set; }
		[Column("register_time")]    public DateTime register_time { get; set; }
	}

	[Table("privacy")]
	public class Privacy
	{
		[Key][Column("id")]   public long      id       { get; set; }
		[Column("duid")]      public string    duid     { get; set; }
		[Column("ldu_time")]  public DateTime? ldu_time { get; set; }
	}

	[Table("dormant")]
	public class Dormant
	{
		[Key][Column("id")]        public long     id           { get; set; }
		[Column("sid")]            public string   sid          { get; set; }
		[Column("aid")]            public string   aid          { get; set; }
		[Column("pid")]            public string   pid          { get; set; }
		[Column("dormant_time")]   public DateTime dormant_time { get; set; }
		[Column("is_dormant")]     public bool     is_dormant   { get; set; }
		[Column("active_time")]    public DateTime active_time  { get; set; }
	}
}
