﻿namespace Nosthy.Blazor.DexieWrapper.JsModule
{
    public interface IModule : IDisposable
    {
        public Task<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, params object[] args);
        public Task InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object[] args);
    }
}
