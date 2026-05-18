<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
</Query>

#load "SnsModel.cs"

// --- Test ---
var s = new Models.Sns.Schedule { id = 1, topic = "all", title = "Update", is_post = false };
s.Dump();
