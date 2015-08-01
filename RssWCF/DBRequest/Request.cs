using System;
using System.ServiceModel;
using RssWCF.RssServiceReference;
using System.Reflection;

namespace RssWCF.DBRequest
{
    public static class Request
    {
        public static void Invoke(Action action)
        {
            string result = String.Empty;
            try
            {
                action();
            }
            catch (TimeoutException ex)
            {
                result = "The service operation timed out. " +
                    ex.Message;
            }
            catch (FaultException<DBRequestFault> ex)
            {
                result = "ProductFault returned: " +
                    ex.Detail.FaultMessage;
            }
            catch (FaultException ex)
            {
                result = "Unknown Fault: " +
                    ex.ToString();
            }
            catch (CommunicationException ex)
            {
                result = "There was a communication problem. " +
                    ex.Message + ex.StackTrace;
            }
            catch (TargetInvocationException tarInvEx)
            {
                result = "There was a TargetInvocation problem. " +
                    tarInvEx.Message + tarInvEx.StackTrace;
            }
            catch (Exception ex)
            {
                result = "Other exception: " + ex.GetType().FullName + " " +
                    ex.Message + ex.StackTrace;
            }
            if (result!=String.Empty)
            {
                ErrorWindow.CreateNew(result);
            }
        }
    }
}
