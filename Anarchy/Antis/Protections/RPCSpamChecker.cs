﻿namespace Antis.Protections
{
    public class RPCSpamChecker : IProtection<string>
    {
        public bool Check(string rpcName)
        {
            return true;
        }

        bool IProtection.Check(object rpcName)
        {
            return true;
        }
    }
}