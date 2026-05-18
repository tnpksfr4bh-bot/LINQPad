<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
</Query>

#load "IapModel.cs"

// --- Test ---
var inv = new Models.Iap.Invoice { invoice_id = 1, product_id = "com.example.gem100", store = 1 };
inv.Dump();
