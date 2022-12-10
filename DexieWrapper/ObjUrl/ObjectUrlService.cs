﻿using Nosthy.Blazor.DexieWrapper.JsModule;
using System.Reflection;

namespace Nosthy.Blazor.DexieWrapper.ObjUrl
{
    public sealed class ObjectUrlService : IAsyncDisposable
    {
        private readonly ObjectUrlJsInterop _objecUrlJsInterop;
        private bool disposed = false;

        public ObjectUrlService(IModuleFactory jsModuleFactory)
        {
            _objecUrlJsInterop = new ObjectUrlJsInterop(jsModuleFactory);
        }

        public async Task<string> Create(byte[] data, CancellationToken cancellationToken = default)
        {
            return await _objecUrlJsInterop.Create(data, cancellationToken);
        }

        public async Task Revoke(string objectUrl, CancellationToken cancellationToken = default)
        {
            await _objecUrlJsInterop.Revoke(objectUrl, cancellationToken);
        }

        public async Task<byte[]> FetchDataNode(string objectUrl, CancellationToken cancellationToken = default)
        {
           return await _objecUrlJsInterop.FetchDataNode(objectUrl, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (!disposed)
            {
                await _objecUrlJsInterop.DisposeAsync();
                disposed = true;
            }
        }
    }
}

