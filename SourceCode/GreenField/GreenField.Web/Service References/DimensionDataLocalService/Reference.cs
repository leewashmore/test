﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GreenField.Web.DimensionDataLocalService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DimensionDataLocalService.ISchema")]
    public interface ISchema {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISchema/GetViews", ReplyAction="http://tempuri.org/ISchema/GetViewsResponse")]
        string[] GetViews(string connectionString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISchema/ListViews", ReplyAction="http://tempuri.org/ISchema/ListViewsResponse")]
        string[] ListViews();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISchema/RunView", ReplyAction="http://tempuri.org/ISchema/RunViewResponse")]
        System.Data.DataSet RunView(string viewName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISchemaChannel : GreenField.Web.DimensionDataLocalService.ISchema, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SchemaClient : System.ServiceModel.ClientBase<GreenField.Web.DimensionDataLocalService.ISchema>, GreenField.Web.DimensionDataLocalService.ISchema {
        
        public SchemaClient() {
        }
        
        public SchemaClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SchemaClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SchemaClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SchemaClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string[] GetViews(string connectionString) {
            return base.Channel.GetViews(connectionString);
        }
        
        public string[] ListViews() {
            return base.Channel.ListViews();
        }
        
        public System.Data.DataSet RunView(string viewName) {
            return base.Channel.RunView(viewName);
        }
    }
}
