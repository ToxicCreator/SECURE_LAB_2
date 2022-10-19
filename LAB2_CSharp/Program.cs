using System.Text;

class Program
{
  static int[]? parts;
  static readonly byte[][] hashes =
  {
    StringHashToByteArray("a2f389fde25ab568a088a6ec1e003823")
    //StringHashToByteArray("3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b"),
    //StringHashToByteArray("74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f")
  };
  static int[] GetParts(int threadCount)
  {
    var part = 26 / threadCount;
    int[] parts = new int[threadCount + 1];
    for (int i = 0; i < threadCount; i++)
    {
      parts[i] = i * part;
    }
    parts[threadCount] = 26;
    return parts;
  }
  static void BrutForce(object? o)
  {
    byte[] hash;
    byte[] word = new byte[5];
    int i = (int)o;
    int start = parts[i];
    int end = parts[i + 1];
    for (int ch = start; ch < end; ch++)
    {
      word[0] = (byte)(97 + ch);
      var sha = System.Security.Cryptography.MD5.Create();
      for (word[1] = 97; word[1] < 123; word[1]++)
        for (word[2] = 97; word[2] < 123; word[2]++)
          for (word[3] = 97; word[3] < 123; word[3]++)
            for (word[4] = 97; word[4] < 123; word[4]++)
            {
              hash = sha.ComputeHash(word);
              if (IsEqualByteArrays(hashes[0], hash))
              {
                Console.WriteLine(
                  Encoding.ASCII.GetString(word)
                    + " => "
                    + BitConverter.ToString(hash)
                );
                return;
              }
            }
    }
  }
  static byte[] StringHashToByteArray(string s)
  {
    return Enumerable.Range(0, s.Length / 2)
      .Select(i => (byte)Convert.ToInt16(s.Substring(i * 2, 2), 16)).ToArray();
  }
  static bool IsEqualByteArrays(byte[] a, byte[] b)
  {
    for (int i = 0; i < 16; i++)
      if (a[i] != b[i])
        return false;
    return true;
  }
  static void Main(string[] args)
  {
    Console.Write("Введите кол-во потоков: ");
    int threadCount = Int16.Parse(Console.ReadLine());
    var watch = System.Diagnostics.Stopwatch.StartNew();

    byte[] word = new byte[5];
    parts = GetParts(threadCount);
    Thread[] threads = new Thread[threadCount];
    for (int i = 0; i < threadCount; i++)
    {
      threads[i] = new Thread((index) =>
      {
        BrutForce(index);
      });
      threads[i].Start(i);
    }
    foreach (var thread in threads)
      thread.Join();
    watch.Stop();
    var elapsedMs = watch.ElapsedMilliseconds / 1000.0;
    Console.WriteLine(elapsedMs);
  }
}