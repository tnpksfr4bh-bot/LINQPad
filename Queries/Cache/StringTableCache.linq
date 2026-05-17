<Query Kind="Statements">
  <Namespace>Common</Namespace>
</Query>

#load "../../Src/Common/StringTableCache.cs"

// --- Test ---
StringTableCache.Set("greeting__KO__KR", "안녕하세요");
StringTableCache.Set("greeting__EN__US", "Hello");
StringTableCache.Set("greeting__ZH__CN", "你好");

Console.WriteLine(StringTableCache.Get("greeting", "ko", "kr")); // 안녕하세요
Console.WriteLine(StringTableCache.Get("greeting", "en", "us")); // Hello
Console.WriteLine(StringTableCache.Get("greeting", "fr", "fr")); // fallback → ko → 안녕하세요
Console.WriteLine(StringTableCache.Get("missing",  "en", "us")); // key not found → "missing"
