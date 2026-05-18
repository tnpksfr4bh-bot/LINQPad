using CSharp.Ulid;
using System.Globalization;
using System.Text;

// Converts between long IDs and ULID strings.
// The long is packed into the lower 8 bytes of the 10-byte randomness section.
public static class UlidConverter
{
	// Generates a new random ULID using the current UTC time.
	public static string New() => Ulid.NewUlid().ToString();

	// Packs a long ID into a ULID at the given ISO-8601 timestamp string.
	public static string FromLong(long id, string isoTimestamp)
	{
		DateTime dt = DateTime.ParseExact(
			isoTimestamp, "yyyy-MM-ddTHH:mm:ss",
			CultureInfo.InvariantCulture,
			DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

		long ms        = new DateTimeOffset(dt).ToUnixTimeMilliseconds();
		byte[] idBytes = BitConverter.GetBytes(id);

		// Build 10-byte randomness: 2 zero bytes + 8 bytes of id (big-endian)
		byte[] rand = new byte[10];
		Array.Copy(idBytes, rand, 8);
		if (BitConverter.IsLittleEndian) Array.Reverse(rand, 0, 8);

		return new Ulid(ms, rand).ToString();
	}

	// Extracts the long ID from a ULID's randomness section (lower 8 bytes).
	public static long ToLong(string ulid)
	{
		Ulid.TryParse(ulid, out Ulid parsed);
		byte[] rand = new byte[]
		{
			parsed.Randomness_0, parsed.Randomness_1,
			parsed.Randomness_2, parsed.Randomness_3,
			parsed.Randomness_4, parsed.Randomness_5,
			parsed.Randomness_6, parsed.Randomness_7,
			parsed.Randomness_8, parsed.Randomness_9,
		};
		// Long is in bytes [2..9] (big-endian, skipping the 2 leading zero bytes)
		byte[] longBytes = new byte[8];
		Array.Copy(rand, 2, longBytes, 0, 8);
		if (BitConverter.IsLittleEndian) Array.Reverse(longBytes);
		return BitConverter.ToInt64(longBytes, 0);
	}

	// Returns the UTC timestamp embedded in a ULID.
	public static DateTime GetTimestamp(string ulid)
	{
		Ulid.TryParse(ulid, out Ulid parsed);
		return DateTimeOffset.FromUnixTimeMilliseconds(parsed.TimeStamp).UtcDateTime;
	}

	// Crockford Base32 encode (ULID alphabet)
	public static string ToBase32(byte[] bytes)
	{
		const string alpha = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
		var sb = new StringBuilder();
		for (int i = 0; i < bytes.Length * 8; i += 5)
		{
			int dual = bytes[i / 8] << 8;
			if (i / 8 + 1 < bytes.Length) dual |= bytes[i / 8 + 1];
			dual = 0x1F & (dual >> (13 - (i % 8)));
			sb.Append(alpha[dual]);
		}
		return sb.ToString();
	}

	// Crockford Base32 decode
	public static byte[] FromBase32(string input)
	{
		const string alpha = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
		byte[] bytes  = new byte[input.Length * 5 / 8];
		int buf = 0, bits = 0, idx = 0;
		foreach (char c in input)
		{
			int v = alpha.IndexOf(c);
			if (bits + 5 >= 8)
			{
				buf    = (buf << 5) | v;
				buf  >>= bits - 3;
				bytes[idx++] = (byte)buf;
				buf   = v;
				bits -= 3;
			}
			else
			{
				buf   = (buf << 5) | v;
				bits += 5;
			}
		}
		return bytes;
	}
}
