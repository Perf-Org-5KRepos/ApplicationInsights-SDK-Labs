// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: TelemetryBatch.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Microsoft.LocalForwarder.Library.Inputs.Contracts {
  /// <summary>
  /// gRPC service to transmit telemetry
  /// </summary>
  public static partial class AITelemetryService
  {
    static readonly string __ServiceName = "contracts.AITelemetryService";

    static readonly grpc::Marshaller<global::Microsoft.LocalForwarder.Library.Inputs.Contracts.TelemetryBatch> __Marshaller_TelemetryBatch = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Microsoft.LocalForwarder.Library.Inputs.Contracts.TelemetryBatch.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Microsoft.LocalForwarder.Library.Inputs.Contracts.AiResponse> __Marshaller_AiResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Microsoft.LocalForwarder.Library.Inputs.Contracts.AiResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::Microsoft.LocalForwarder.Library.Inputs.Contracts.TelemetryBatch, global::Microsoft.LocalForwarder.Library.Inputs.Contracts.AiResponse> __Method_SendTelemetryBatch = new grpc::Method<global::Microsoft.LocalForwarder.Library.Inputs.Contracts.TelemetryBatch, global::Microsoft.LocalForwarder.Library.Inputs.Contracts.AiResponse>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "SendTelemetryBatch",
        __Marshaller_TelemetryBatch,
        __Marshaller_AiResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Microsoft.LocalForwarder.Library.Inputs.Contracts.TelemetryBatchReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of AITelemetryService</summary>
    public abstract partial class AITelemetryServiceBase
    {
      public virtual global::System.Threading.Tasks.Task SendTelemetryBatch(grpc::IAsyncStreamReader<global::Microsoft.LocalForwarder.Library.Inputs.Contracts.TelemetryBatch> requestStream, grpc::IServerStreamWriter<global::Microsoft.LocalForwarder.Library.Inputs.Contracts.AiResponse> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for AITelemetryService</summary>
    public partial class AITelemetryServiceClient : grpc::ClientBase<AITelemetryServiceClient>
    {
      /// <summary>Creates a new client for AITelemetryService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public AITelemetryServiceClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for AITelemetryService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public AITelemetryServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected AITelemetryServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected AITelemetryServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual grpc::AsyncDuplexStreamingCall<global::Microsoft.LocalForwarder.Library.Inputs.Contracts.TelemetryBatch, global::Microsoft.LocalForwarder.Library.Inputs.Contracts.AiResponse> SendTelemetryBatch(grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return SendTelemetryBatch(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncDuplexStreamingCall<global::Microsoft.LocalForwarder.Library.Inputs.Contracts.TelemetryBatch, global::Microsoft.LocalForwarder.Library.Inputs.Contracts.AiResponse> SendTelemetryBatch(grpc::CallOptions options)
      {
        return CallInvoker.AsyncDuplexStreamingCall(__Method_SendTelemetryBatch, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override AITelemetryServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new AITelemetryServiceClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(AITelemetryServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_SendTelemetryBatch, serviceImpl.SendTelemetryBatch).Build();
    }

  }
}
#endregion