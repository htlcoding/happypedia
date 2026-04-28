using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;

namespace Soltani_Chinesischer_Restsatz
{
    class RsaCrt
    {
        // Erweiterter Euklidischer Algorithmus
        // Gibt zurueck: ggT, x, y  =>  a*x + b*y = ggT
        static (BigInteger ggT, BigInteger x, BigInteger y) ErweiterterEuklid(BigInteger a, BigInteger b)
        {
            BigInteger alterRest = a, rest = b;
            BigInteger alterX = 1, x = 0;
            BigInteger alterY = 0, y = 1;

            while (rest != 0)
            {
                BigInteger q = alterRest / rest;

                (alterRest, rest) = (rest, alterRest - q * rest);
                (alterX, x)       = (x,    alterX    - q * x);
                (alterY, y)       = (y,    alterY    - q * y);
            }

            return (alterRest, alterX, alterY);
        }

        // Berechnet a^(-1) mod m
        // Beispiel: ModularesInverses(13, 7) = 6, weil 13 * 6 = 78 = 11*7 + 1
        static BigInteger ModularesInverses(BigInteger a, BigInteger m)
        {
            var (ggT, x, _) = ErweiterterEuklid(a, m);

            if (ggT != 1)
                throw new Exception($"Kein Inverses: ggT({a},{m}) = {ggT}");

            // sicherstellen dass positiv
            BigInteger result = x % m;
            if (result < 0) result += m;
            return result;
        }

        // Normale RSA Entschluesselung: m = c^d mod n
        static BigInteger EntschluesselungStandard(BigInteger c, BigInteger d, BigInteger n)
        {
            return BigInteger.ModPow(c, d, n);
        }

        // CRT Entschluesselung (schneller, weil zwei kleine Rechnungen statt einer grossen)
        // dp   = d mod (p-1)
        // dq   = d mod (q-1)
        // qInv = q^(-1) mod p
        static BigInteger EntschluesselungCRT(BigInteger c, BigInteger dp, BigInteger dq,
                                              BigInteger qInv, BigInteger p, BigInteger q)
        {
            // im "p-Universum" entschluesseln
            BigInteger m1 = BigInteger.ModPow(c, dp, p);

            // im "q-Universum" entschluesseln
            BigInteger m2 = BigInteger.ModPow(c, dq, q);

            // Garner's Algorithmus: beide Teile zusammenfuehren
            BigInteger diff = m1 - m2;
            if (diff < 0) diff += p;  // negative Reste verhindern

            BigInteger h = (qInv * diff) % p;
            BigInteger m = m2 + h * q;

            return m;
        }

        // Demo aus dem Skript: p=5, q=11, d=27, c=49 => m=14
        static void DemoSkript()
        {
            Console.WriteLine("--- Demo Skript: p=5, q=11 ---");

            BigInteger p = 5, q = 11, d = 27, c = 49;

            BigInteger dp   = d % (p - 1);          // 27 mod 4 = 3
            BigInteger dq   = d % (q - 1);          // 27 mod 10 = 7
            BigInteger qInv = ModularesInverses(q, p); // 11^(-1) mod 5 = 1

            Console.WriteLine($"dp={dp}, dq={dq}, qInv={qInv}");
            Console.WriteLine($"Ergebnis: {EntschluesselungCRT(c, dp, dq, qInv, p, q)}  (erwartet: 14)");
            Console.WriteLine();
        }

        // Paper-Beispiel: p=7, q=13, m=20
        static void DemoPapier()
        {
            Console.WriteLine("--- Paper-Beispiel: p=7, q=13, m=20 ---");

            BigInteger p = 7, q = 13;
            BigInteger n = p * q;        // 91
            BigInteger e = 5, d = 29;    // 5*29=145=2*72+1 => d*e ≡ 1 mod phi(n)
            BigInteger m = 20;

            BigInteger c = BigInteger.ModPow(m, e, n);
            Console.WriteLine($"Verschluesselt: {m}^{e} mod {n} = {c}");

            BigInteger dp   = d % (p - 1);            // 29 mod 6 = 5
            BigInteger dq   = d % (q - 1);            // 29 mod 12 = 5
            BigInteger qInv = ModularesInverses(q, p); // 13^(-1) mod 7 = 6

            Console.WriteLine($"dp={dp}, dq={dq}, qInv={qInv}");
            Console.WriteLine($"Ergebnis: {EntschluesselungCRT(c, dp, dq, qInv, p, q)}  (erwartet: 20)");
            Console.WriteLine();
        }

        // 2048-Bit Benchmark: Standard vs CRT
        static void Benchmark()
        {
            Console.WriteLine("--- Benchmark: 2048-Bit RSA ---");

            using var rsa = RSA.Create(2048);
            RSAParameters key = rsa.ExportParameters(includePrivateParameters: true);

            // Byte-Arrays in BigInteger umwandeln (fuehrende 0x00 fuer positives Vorzeichen)
            BigInteger n    = new BigInteger(Pad(key.Modulus!),   isUnsigned: true, isBigEndian: true);
            BigInteger d    = new BigInteger(Pad(key.D!),         isUnsigned: true, isBigEndian: true);
            BigInteger p    = new BigInteger(Pad(key.P!),         isUnsigned: true, isBigEndian: true);
            BigInteger q    = new BigInteger(Pad(key.Q!),         isUnsigned: true, isBigEndian: true);
            BigInteger dp   = new BigInteger(Pad(key.DP!),        isUnsigned: true, isBigEndian: true);
            BigInteger dq   = new BigInteger(Pad(key.DQ!),        isUnsigned: true, isBigEndian: true);
            BigInteger qInv = new BigInteger(Pad(key.InverseQ!),  isUnsigned: true, isBigEndian: true);

            BigInteger plaintext  = n - 42;
            BigInteger ciphertext = BigInteger.ModPow(plaintext, 65537, n);

            const int RUNS = 20;

            // Standard messen
            var sw = Stopwatch.StartNew();
            BigInteger resStd = BigInteger.Zero;
            for (int i = 0; i < RUNS; i++)
                resStd = EntschluesselungStandard(ciphertext, d, n);
            sw.Stop();
            double msStd = sw.Elapsed.TotalMilliseconds / RUNS;

            // CRT messen
            sw.Restart();
            BigInteger resCRT = BigInteger.Zero;
            for (int i = 0; i < RUNS; i++)
                resCRT = EntschluesselungCRT(ciphertext, dp, dq, qInv, p, q);
            sw.Stop();
            double msCRT = sw.Elapsed.TotalMilliseconds / RUNS;

            bool korrekt = resStd == resCRT && resStd == plaintext;

            Console.WriteLine($"Standard: {msStd:F3} ms");
            Console.WriteLine($"CRT:      {msCRT:F3} ms");
            Console.WriteLine($"Speedup:  {msStd / msCRT:F2}x");
            Console.WriteLine($"Korrekt:  {(korrekt ? "Ja" : "FEHLER!")}");
        }

        // fuehrendes 0x00-Byte damit BigInteger positiv bleibt
        static byte[] Pad(byte[] b)
        {
            byte[] res = new byte[b.Length + 1];
            Buffer.BlockCopy(b, 0, res, 1, b.Length);
            return res;
        }

        static void Main()
        {
            DemoSkript();
            DemoPapier();
            Benchmark();
        }
    }
}