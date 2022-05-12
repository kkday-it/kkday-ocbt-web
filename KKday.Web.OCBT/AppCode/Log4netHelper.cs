using System;
using log4net; 

namespace KKday.Web.OCBT.AppCode
{
    public class Log4netHelper
    {
        public readonly ILog logger;

        public Log4netHelper(ILog _logger, string _service_name)
        {
            logger = _logger;
            log4net.LogicalThreadContext.Properties["service_name"] = _service_name;
            log4net.LogicalThreadContext.Properties["log_label"] = "TRACE";
        }

        public void Debug(object message, Exception exception, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Debug(message,exception);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Debug Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void Debug(object message, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Debug(message);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Debug Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.DebugFormat(provider, format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Debug Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void DebugFormat(string fotmat, object org0, object org1, object org2)
        {
            try
            {
                logger.DebugFormat(fotmat, org0, org1, org2);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Debug Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void DebugFormat(string fotmat, object org0, object org1)
        {
            try
            {
                logger.DebugFormat(fotmat, org0, org1);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Debug Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void DebugFormat(string format, object arg0)
        {
            try
            {
                logger.DebugFormat(format, arg0);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Debug Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.DebugFormat(format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Debug Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void Error(object message, Exception exception, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Error(message, exception);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Error Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void Error(object message, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Error(message);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Error Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.ErrorFormat(provider, format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Error Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void ErrorFormat(string fotmat, object org0, object org1, object org2)
        {
            try
            {
                logger.ErrorFormat(fotmat, org0, org1, org2);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Error Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void ErrorFormat(string fotmat, object org0, object org1)
        {
            try
            {
                logger.ErrorFormat(fotmat, org0, org1);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Error Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void ErrorFormat(string format, object arg0)
        {
            try
            {
                logger.ErrorFormat(format, arg0);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Error Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void ErrorFormat( string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.ErrorFormat(format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Error Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void Fatal(object message, Exception exception, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Fatal(message, exception);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Fatal Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void Fatal(object message, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Fatal(message);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Fatal Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.FatalFormat(provider, format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Fatal Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void FatalFormat(string fotmat, object org0, object org1, object org2)
        {
            try
            {
                logger.FatalFormat(fotmat, org0, org1, org2);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Fatal Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void FatalFormat(string fotmat, object org0, object org1)
        {
            try
            {
                logger.FatalFormat(fotmat, org0, org1);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Fatal Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void FatalFormat(string format, object arg0)
        {
            try
            {
                logger.FatalFormat(format, arg0);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Fatal Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.FatalFormat(format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Fatal Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }


        public void Info(object message, Exception exception, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Info(message, exception);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Info Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void Info(object message, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Info(message);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Info Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.InfoFormat(provider, format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Info Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void InfoFormat(string fotmat, object org0, object org1 ,object org2)
        {
            try
            {
                logger.InfoFormat(fotmat, org0, org1, org2);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Info Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void InfoFormat(string fotmat, object org0, object org1)
        {
            try
            {
                logger.InfoFormat(fotmat, org0, org1);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Info Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void InfoFormat(string format, object arg0)
        {
            try
            {
                logger.InfoFormat(format, arg0);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Info Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.InfoFormat(format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Info Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }


        public void Warn(object message, Exception exception, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Warn(message, exception);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Warn Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void Warn(object message, string requestUuid = null)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = requestUuid;
                logger.Warn(message);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Warn Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.WarnFormat(provider, format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Warn Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void WarnFormat(string fotmat, object org0, object org1, object org2)
        {
            try
            {
                logger.WarnFormat(fotmat, org0, org1, org2);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Warn Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void WarnFormat(string fotmat, object org0, object org1)
        {
            try
            {
                logger.WarnFormat(fotmat, org0, org1);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Warn Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void WarnFormat(string format, object arg0)
        {
            try
            {
                logger.WarnFormat(format, arg0);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Warn Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            try
            {
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
                logger.WarnFormat(format, args);
                log4net.LogicalThreadContext.Properties["requestuuid"] = null;
            }
            catch (Exception ex)
            {
                Website.Instance.loggerOrg.Debug("Log4netHelper Warn Error !" + ex.ToString() + ex.StackTrace.ToString());
            }
        }

    }
}
