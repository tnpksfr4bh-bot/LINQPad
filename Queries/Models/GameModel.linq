<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
</Query>

#load "GameModel.cs"

// --- Test ---
var e = new Models.Game.GameDataEntity { id = 1, data = "{}", revision = 1 };
e.Dump();
