<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
</Query>

#load "ServiceModel.cs"

// --- Test ---
var session = new Models.Service.PlayerSession { sid = "s-001", aid = "a-001", status = "" };
session.Dump();
