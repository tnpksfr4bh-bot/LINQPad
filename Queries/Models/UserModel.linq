<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
</Query>

#load "UserModel.cs"

// --- Test ---
var p = new Models.User.Player { pid = "p-001", platform = "ios", uid = "u-001", name = "Alice" };
p.Dump();
