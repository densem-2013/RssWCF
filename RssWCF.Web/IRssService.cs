using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.CompilerServices;

namespace RssWCF.Web
{
    [ServiceContract(Namespace = "")]
    public interface IRssService
    {
        [OperationContract]
        [FaultContract(typeof(DBRequestFault))]
        List<Feed> GetFeeds();

        [OperationContract]
        [FaultContract(typeof(DBRequestFault))]
        void AddFeed(string name, string url);

        [OperationContract]
        [FaultContract(typeof(DBRequestFault))]
        void RemoveFeed(Guid feed_id);

        [OperationContract]
        [FaultContract(typeof(DBRequestFault))]
        void FetchAllFeedNews();

        [OperationContract]
        [FaultContract(typeof(DBRequestFault))]
        Boolean GetNews();

        [OperationContract]
        [FaultContract(typeof(DBRequestFault))]
        void RemoveNews(Guid item_id);

        [OperationContract]
        [FaultContract(typeof(DBRequestFault))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void RemoveAllNews();
    }
    [DataContract]
    public class DBRequestFault
    {
        public DBRequestFault(string msg)
        {
            FaultMessage = msg;
        }

        [DataMember]
        public string FaultMessage;
    }

}
