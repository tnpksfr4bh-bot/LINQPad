<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
</Query>

#load "CmsModel.cs"

// --- Test ---
var auth = new Models.Cms.Auth { id = "admin-001", email = "admin@example.com", level = 99 };
auth.Dump();
