using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Net;

namespace TracerX
{
    public class ClientBaseEx<TChannel> : ClientBase<TChannel>, IDisposable where TChannel : class
    {
        /// <summary>
        /// Allows you to change the remote host and/or port before connecting to the WCF service.
        /// The parameter's format is 'hostNameOrAddress[:optionalPort]'.
        /// </summary>
        public void SetHost(string hostAndOptionalPort)
        {
            if (State == CommunicationState.Created)
            {
                // Get the current address from the endpoint and change only the Host
                // and possibly the port.

                var u = new UriBuilder(this.Endpoint.Address.Uri);
                string[] parts = hostAndOptionalPort.Split(':');

                if (parts.Length == 2)
                {
                    int port;
                    u.Host = parts[0].Trim();

                    if (int.TryParse(parts[1].Trim(), out port))
                    {
                        u.Port = port;
                    }
                    else
                    {
                        throw new FormatException("Invalid format hostAndOptionalPort.  Correct format is 'hostNameOrAddress[:optionalPort]'.");
                    }
                }
                else if (parts.Length > 2)
                {
                    throw new FormatException("Invalid format hostAndOptionalPort.  Correct format is 'hostNameOrAddress[:optionalPort]'.");
                }
                else
                {
                    u.Host = hostAndOptionalPort;
                }

                this.Endpoint.Address = new System.ServiceModel.EndpointAddress(u.Uri, this.Endpoint.Address.Identity, this.Endpoint.Address.Headers);
                ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            }
            else
            {
                throw new InvalidOperationException("The proxy must be in the Created state to change the host or port.  Current state is " + State);
            }
        }

        /// <summary>
        /// Allows you to change the client credentials before connecting to the WCF service.
        /// </summary>
        public void SetCredentials(NetworkCredential credentials)
        {
            if (State == CommunicationState.Created)
            {
                if (credentials != null)
                {
                    ClientCredentials.Windows.ClientCredential = credentials;
                }
            }
            else
            {
                throw new InvalidOperationException("The proxy must be in the Created state to change the credentials.  Current state is " + State);
            }
        }

        // This fixes the "broken WCF client IDisposable" problem.
        public void Dispose()
        {
            try
            {
                if (State != CommunicationState.Faulted)
                {
                    Close();
                }
                else
                {
                    Abort();
                }
            }
            catch (CommunicationException)
            {
                Abort();
            }
            catch (TimeoutException)
            {
                Abort();
            }
            catch (Exception)
            {
                Abort();
                throw;
            }
        }

    }

    public class DuplexClientBaseEx<TChannel> : DuplexClientBase<TChannel>, IDisposable where TChannel : class
    {
        public DuplexClientBaseEx(object callbackObject)
            :base(callbackObject)
        {
        }

        /// <summary>
        /// Allows you to change the remote host and/or port before connecting to the WCF service.
        /// The parameter's format is 'hostNameOrAddress[:optionalPort]'.
        /// </summary>
        public void SetHost(string hostAndOptionalPort)
        {
            if (State == CommunicationState.Created)
            {
                var u = new UriBuilder(this.Endpoint.Address.Uri);
                string[] parts = hostAndOptionalPort.Split(':');

                if (parts.Length == 2)
                {
                    int port;
                    u.Host = parts[0].Trim();

                    if (int.TryParse(parts[1].Trim(), out port))
                    {
                        u.Port = port;
                    }
                    else
                    {
                        throw new FormatException("Invalid format hostAndOptionalPort.  Correct format is 'hostNameOrAddress[:optionalPort]'.");
                    }
                }
                else if (parts.Length > 2)
                {
                    throw new FormatException("Invalid format hostAndOptionalPort.  Correct format is 'hostNameOrAddress[:optionalPort]'.");
                }
                else
                {
                    u.Host = hostAndOptionalPort;
                }

                this.Endpoint.Address = new System.ServiceModel.EndpointAddress(u.Uri, this.Endpoint.Address.Identity, this.Endpoint.Address.Headers);
            }
            else
            {
                throw new InvalidOperationException("The proxy must be in the Created state to change the host or port.  Current state is " + State);
            }
        }

        /// <summary>
        /// Allows you to change the client credentials before connecting to the WCF service.
        /// </summary>
        public void SetCredentials(NetworkCredential credentials)
        {
            if (State == CommunicationState.Created)
            {
                if (credentials != null)
                {
                    ClientCredentials.Windows.ClientCredential = credentials;
                }
            }
            else
            {
                throw new InvalidOperationException("The proxy must be in the Created state to change the credentials.  Current state is " + State);
            }
        }

        // This fixes the "broken WCF client IDisposable" problem.
        public void Dispose()
        {
            try
            {
                if (State != CommunicationState.Faulted)
                {
                    Close();
                }
                else
                {
                    Abort();
                }
            }
            catch (CommunicationException)
            {
                Abort();
            }
            catch (TimeoutException)
            {
                Abort();
            }
            catch (Exception)
            {
                Abort();
                throw;
            }
        }

    }

}
