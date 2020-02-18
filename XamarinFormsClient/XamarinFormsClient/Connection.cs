using System;    
using System.Collections.Generic;    
using System.Net.Sockets;    
    
namespace XamarinFormsClient.Client    
{    
    public class Connection    
    {    
        private static Connection _instance;    
        public static Connection Instance    
        {    
            get    
            {    
                if (_instance == null) _instance = new Connection();    
                return _instance;    
            }    
        }    
        public TcpClient client { get; set; }    
    }    
}    