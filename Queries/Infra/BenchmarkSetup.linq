<Query Kind="Program">
  <NuGetReference Version="0.13.9">BenchmarkDotNet</NuGetReference>
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>BenchmarkDotNet.Configs</Namespace>
  <Namespace>BenchmarkDotNet.Diagnosers</Namespace>
  <Namespace>BenchmarkDotNet.Jobs</Namespace>
  <Namespace>BenchmarkDotNet.Running</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

// #LINQPad optimize+   // Uncomment to enable JIT optimizations during benchmarking

// --- Test ---
// Run via Query → Benchmark Selected Code (Ctrl+Shift+B), or call RunBenchmarks() from Main.
void Main()
{
	// Quick run (fewer iterations, faster feedback):
	var config = ManualConfig.CreateEmpty()
		.AddJob(Job.ShortRun)
		.AddDiagnoser(MemoryDiagnoser.Default)
		.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray())
		.AddValidator(DefaultConfig.Instance.GetValidators().ToArray())
		.WithOptions(ConfigOptions.DisableOptimizationsValidator);

	BenchmarkRunner.Run(GetType().Assembly, config);
}

// ---
// Add your own [Benchmark] methods below this line.
// Each method will appear as a row in the results table.

byte[]        _buffer = new byte[10_000];
HashAlgorithm _sha1   = SHA1.Create();
HashAlgorithm _sha256 = SHA256.Create();

[Benchmark(Baseline = true)]
public void HashSHA1()   => _sha1.ComputeHash(_buffer);

[Benchmark]
public void HashSHA256() => _sha256.ComputeHash(_buffer);
