<Query Kind="Statements">
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "AppConfigModel.cs"

// --- Test ---
var bundle = new Models.App.Bundle { aid = "a-001", bundle_id = "com.example.app", status = "live" };
bundle.Dump();
var lt = new Models.App.LocaleText { key = "hello", ko_kr = "안녕", en_us = "Hello" };
Console.WriteLine(lt.GetText("ko_kr")); // 안녕
