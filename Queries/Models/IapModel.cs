using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Iap
{
	[Table("invoice")]
	public class Invoice
	{
		[Key][Column("invoice_id")]                                        public long     invoice_id     { get; set; }
		[Column("sid")]                                                    public string   sid            { get; set; }
		[Column("aid")]                                                    public string   aid            { get; set; }
		[Column("pid")]                                                    public string   pid            { get; set; }
		[Column("user_id")]                                                public string   user_id        { get; set; }
		[Column("bundle_name")]                                            public string   bundle_name    { get; set; }
		[Column("invoice_time")]                                           public DateTime invoice_time   { get; set; }
		[Column("product_id")]                                             public string   product_id     { get; set; }
		[Column("price")]                                                  public long     price          { get; set; }
		[Column("currency")]                                               public string   currency       { get; set; }
		[Column("store")]                                                  public int      store          { get; set; }
		[Column("order_id")]                                               public string   order_id       { get; set; }
		[Column("purchase_token")]                                         public string   purchase_token { get; set; }
		[Column("validate")]                                               public int      validate       { get; set; }
		[Column("valid_count")]                                            public int      valid_count    { get; set; }
		[Column("verify")]                                                 public int      verify         { get; set; }
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("sign")] public string? sign        { get; set; }
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("lid")]  public string? lid         { get; set; }
	}

	[Table("invoice")]
	public class InvoiceAdmin
	{
		public long     invoice_id     { get; set; }
		public string   sid            { get; set; }
		public string   aid            { get; set; }
		public string   pid            { get; set; }
		public string   user_id        { get; set; }
		public string   bundle_name    { get; set; }
		public DateTime invoice_time   { get; set; }
		public string   product_id     { get; set; }
		public long     price          { get; set; }
		public string   currency       { get; set; }
		public int      store          { get; set; }
		public string   order_id       { get; set; }
		public int      validate       { get; set; }
		public int      valid_count    { get; set; }
		public int      verify         { get; set; }
		public string   sign           { get; set; }
		public string   lid            { get; set; }
	}

	[Table("refund")]
	public class Refund
	{
		[Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("id")]
		public long     id                       { get; set; }
		[Column("bundle_id")]                    public string    bundle_id                { get; set; }
		[Column("original_transaction_id")]      public string    original_transaction_id  { get; set; }
		[Column("transaction_id")]               public string    transaction_id           { get; set; }
		[Column("product_id")]                   public string    product_id               { get; set; }
		[Column("is_refund")]                    public bool?     is_refund                { get; set; }
		[Column("refund_time")]                  public DateTime? refund_time              { get; set; }
	}

	[Table("refund")]
	public class RefundAdmin
	{
		[Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long     id                       { get; set; }
		public string   bundle_id                { get; set; }
		public string   original_transaction_id  { get; set; }
		public string   transaction_id           { get; set; }
		public string   product_id               { get; set; }
		public DateTime purchase_date            { get; set; }
		public DateTime original_purchase_date   { get; set; }
		public int      quantity                 { get; set; }
		public string   type                     { get; set; }
		public string   in_app_ownership_type    { get; set; }
		public DateTime signed_date              { get; set; }
		public string   environment              { get; set; }
		public int?     revocation_reason        { get; set; }
		public DateTime? revocation_date         { get; set; }
		public bool?    is_refund                { get; set; }
		public DateTime? refund_time             { get; set; }
	}

	[Table("void_purchase")]
	public class VoidPurchase
	{
		[Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("id")]
		public long     id             { get; set; }
		[Column("bundle_id")]          public string    bundle_id      { get; set; }
		[Column("order_id")]           public string    order_id       { get; set; }
		[Column("purchase_token")]     public string    purchase_token { get; set; }
		[Column("is_refund")]          public bool?     is_refund      { get; set; }
		[Column("refund_time")]        public DateTime? refund_time    { get; set; }
	}

	[Table("void_purchase")]
	public class VoidPurchaseAdmin
	{
		[Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long     id             { get; set; }
		public string   bundle_id      { get; set; }
		public string   order_id       { get; set; }
		public DateTime purchase_time  { get; set; }
		public DateTime voided_time    { get; set; }
		public int      voided_reason  { get; set; }
		public string   purchase_token { get; set; }
		public bool?    is_refund      { get; set; }
		public DateTime? refund_time   { get; set; }
	}

	[Table("appstore_notification")]
	public class AppStoreNotification
	{
		[Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long     id                       { get; set; }
		public string   notification_uuid        { get; set; }
		public string   notification_type        { get; set; }
		public string   version                  { get; set; }
		public bool     is_alert                 { get; set; }
		public DateTime signed_date              { get; set; }
		public long     apple_id                 { get; set; }
		public string   bundle_id                { get; set; }
		public string   bundle_version           { get; set; }
		public string   environment              { get; set; }
		public string   original_transaction_id  { get; set; }
		public string   transaction_id           { get; set; }
	}
}
