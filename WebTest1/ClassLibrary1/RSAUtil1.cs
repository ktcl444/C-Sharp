using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Threading;

/// <summary>
///RSAUtil1 的摘要说明
/// </summary>
public class RSAUtil1
{
    public RSAUtil1()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //



    }

    public static string Test()
    {
        Thread.Sleep(10000);
        return "Sleep End";
    }

    private const string publicKey = "<RSAKeyValue><Modulus>mE8PFoNc6IoMFam4zCquflCkquFB823eNc6lob2yRqSHx8LFMCpttcF2zEZ5I3uSiOGBP8aijTjD8JSk7TvU+EtZfP04hL+nARAGyQM1u+FiabsBOQlkDbOq59dkThmjSPdHw0x/igRdFHe1Uwru4nRbKs1OWczzUsJWWDLV6+0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    //签名验证
    public static bool SignatureDeformatter(string strDeformatter, string strDeformatterData)
    {
        string strHashByteDeformatter = string.Empty;
        if (GetHash(strDeformatter, ref strHashByteDeformatter))
        {
            return SignatureDeformatterByHash(publicKey, strHashByteDeformatter, strDeformatterData);

        }
        return false;
    }

    //获取 HASH 描述表
    public static bool GetHash(string strSource, ref string strHashData)
    {
        try
        {
            //从字符串中取得hash描述
            byte[] buffer = null;
            byte[] hashdata = null;
            HashAlgorithm md5 = HashAlgorithm.Create("md5");
            buffer = System.Text.Encoding.Default.GetBytes(strSource);
            hashdata = md5.ComputeHash(buffer);
            strHashData = Convert.ToBase64String(hashdata);
            return true;
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }

    //签名验证
    private static bool SignatureDeformatterByHash(string strPublicKey, string strHashByteDeformatter, string strDeformatterData)
    {
        try
        {
            byte[] deformatterdata = null;
            byte[] hashbytedeformatter = null;
            hashbytedeformatter = Convert.FromBase64String(strHashByteDeformatter);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strPublicKey);
            RSAPKCS1SignatureDeformatter rsadeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsadeformatter.SetHashAlgorithm("md5");
            deformatterdata = Convert.FromBase64String(strDeformatterData);
            if ((rsadeformatter.VerifySignature(hashbytedeformatter, deformatterdata)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
