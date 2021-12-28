﻿using DexieWrapper.JsInterop;

namespace DexieWrapper.Database
{
    public class Collection<T>
    {
        protected Db Db = null!;
        protected CommandExecuterJsInterop CommandExecuterJsInterop = null!;
        protected string StoreName = null!;
        protected virtual List<Command> CurrentCommands { get; } = new List<Command>();

        protected Collection()
        {
        }

        public Collection(Db db, string storeName, CommandExecuterJsInterop commandExecuterJsInterop)
        {
            Init(db, storeName, commandExecuterJsInterop);
        }

        public void Init(Db db, string storeName, CommandExecuterJsInterop commandExecuterJsInterop)
        {
            Db = db;
            StoreName = storeName;
            CommandExecuterJsInterop = commandExecuterJsInterop;
        }

        public void AddCommand(string command, params object?[] parameters)
        {
            CurrentCommands.Add(new Command(command, parameters));
        }

        public async Task<T[]> ToArray()
        {
            return await Execute<T[]>("toArray");
        }

        public async Task<List<T>> ToList()
        {
            return await Execute<List<T>>("toArray");
        }

        protected async Task<TRet> Execute<TRet>(string command, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            await Db.Init();

            if (Db.DbJsObjectRef != null)
            {
                return await CommandExecuterJsInterop.Execute<TRet>(Db.DbJsObjectRef, StoreName, commands);
            }

            return await CommandExecuterJsInterop.InitDbAndExecute<TRet>(Db.DbDefinition, StoreName, commands);
        }

        protected async Task ExecuteNonQuery(string command, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            await Db.Init();

            if (Db.DbJsObjectRef != null)
            {
                await CommandExecuterJsInterop.ExecuteNonQuery(Db.DbJsObjectRef, StoreName, commands);
            }
            else
            {
                await CommandExecuterJsInterop.InitDbAndExecuteNonQuery(Db.DbDefinition, StoreName, commands);
            }
        }
    }
}
