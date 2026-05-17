<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
</Query>

// --- Test ---
var s = new Models.Sns.Schedule { id = 1, topic = "all", title = "Update", is_post = false };
s.Dump();

// ---

namespace Models.Sns
{
	[Table("schedule")]
	public class Schedule
	{
		[Key][Column("id")]       public int      id        { get; set; }
		[Column("topic")]         public string   topic     { get; set; }
		[Column("title")]         public string   title     { get; set; }
		[Column("body")]          public string   body      { get; set; }
		[Column("image")]         public string   image     { get; set; }
		[Column("post_time")]     public DateTime post_time { get; set; }
		[Column("is_post")]       public bool     is_post   { get; set; }
	}
}
