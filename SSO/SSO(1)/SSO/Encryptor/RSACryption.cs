/*----------------------------------------------------------------
// Copyright (C) 2007 ���ϻ��� ��Ȩ���С� 
//
// �ļ�����RSACryption.cs
// �ļ�����������
// 
// ������ʶ��jillzhang
// �޸ı�ʶ��
// �޸�������
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace SSO
{
    /// <summary> 
    /// RSA���ܽ��ܼ�RSAǩ������֤
    /// </summary> 
    public class RSACryption
    {
        public RSACryption()
        {
        }


        #region RSA ���ܽ���

        #region RSA ����Կ����

        /// <summary>
        /// RSA ����Կ���� ����˽Կ �͹�Կ 
        /// </summary>
        /// <param name="xmlKeys"></param>
        /// <param name="xmlPublicKey"></param>
        public void RSAKey(out string privateKey, out string publicKey)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            privateKey = provider.ToXmlString(true);
            publicKey = provider.ToXmlString(false);
        }
        #endregion

        #region RSA�ļ��ܺ���
        //############################################################################## 
        //RSA ��ʽ���� 
        //˵��KEY������XML����ʽ,���ص����ַ��� 
        //����һ����Ҫ˵�������ü��ܷ�ʽ�� ���� ���Ƶģ��� 
        //############################################################################## 

        //RSA�ļ��ܺ���  string
        public string RSAEncrypt(string publicKey, string m_strEncryptString)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(m_strEncryptString);
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(publicKey);
            return Convert.ToBase64String(provider.Encrypt(bytes, false));
        }
        #endregion

        #region RSA�Ľ��ܺ���
        //RSA�Ľ��ܺ���  string
        public string RSADecrypt(string privateKey, string m_strDecryptString)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(privateKey);
            byte[] rgb = Convert.FromBase64String(m_strDecryptString);
            byte[] bytes = provider.Decrypt(rgb, false);
            return encoding.GetString(bytes);
        }  
        #endregion

        #endregion

        #region RSA����ǩ��

        #region ��ȡHash������
        //��ȡHash������ 
        public bool GetHash(string m_strSource, ref byte[] HashData)
        {
            //���ַ�����ȡ��Hash���� 
            byte[] Buffer;
            System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
            Buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(m_strSource);
            HashData = MD5.ComputeHash(Buffer);

            return true;
        }

        //��ȡHash������ 
        public bool GetHash(string m_strSource, ref string strHashData)
        {

            //���ַ�����ȡ��Hash���� 
            byte[] Buffer;
            byte[] HashData;
            System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
            Buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(m_strSource);
            HashData = MD5.ComputeHash(Buffer);

            strHashData = Convert.ToBase64String(HashData);
            return true;

        }

        //��ȡHash������ 
        public bool GetHash(System.IO.FileStream objFile, ref byte[] HashData)
        {

            //���ļ���ȡ��Hash���� 
            System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
            HashData = MD5.ComputeHash(objFile);
            objFile.Close();

            return true;

        }

        //��ȡHash������ 
        public bool GetHash(System.IO.FileStream objFile, ref string strHashData)
        {

            //���ļ���ȡ��Hash���� 
            byte[] HashData;
            System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
            HashData = MD5.ComputeHash(objFile);
            objFile.Close();

            strHashData = Convert.ToBase64String(HashData);

            return true;

        }
        #endregion    
    
        #region RSAǩ��
        //RSAǩ�� 
        public bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref string m_strEncryptedSignatureData)
        {

            byte[] HashbyteSignature;
            byte[] EncryptedSignatureData;

            HashbyteSignature = Convert.FromBase64String(m_strHashbyteSignature);
            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPrivate);
            System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);
            //����ǩ�����㷨ΪMD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //ִ��ǩ�� 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

            m_strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);

            return true;

        }
        #endregion

        #region RSA ǩ����֤     
        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, string p_strDeformatterData)
        {

            byte[] DeformatterData;
            byte[] HashbyteDeformatter;

            HashbyteDeformatter = Convert.FromBase64String(p_strHashbyteDeformatter);
            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPublic);
            System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
            //ָ�����ܵ�ʱ��HASH�㷨ΪMD5 
            RSADeformatter.SetHashAlgorithm("MD5");

            DeformatterData = Convert.FromBase64String(p_strDeformatterData);

            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        #endregion


        #endregion

    }
}
