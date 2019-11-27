using Microsoft.AspNetCore.SignalR;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoadFlow.Business.SignalR
{
    public class SignalRHub : Hub
    {
        // Fields
        private static IHubContext<SignalRHub> _hubContext;

        // Methods
        [CompilerGenerated, DebuggerHidden]
        private Task s_n__0()
        {
            return base.OnConnectedAsync();
        }

        [CompilerGenerated, DebuggerHidden]
        private Task  s_n__1(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public static void Configure(IHubContext<SignalRHub> accessor)
        {
            _hubContext = accessor;
        }

        [AsyncStateMachine((typeof(OnConnectedAsyncd__3)))]
        public override Task OnConnectedAsync()
        {
            OnConnectedAsyncd__3   d__=new OnConnectedAsyncd__3();
            d__.s_4__this = this;
            d__.s_t__builder = AsyncTaskMethodBuilder.Create();
            d__.s_1__state = -1;
            d__.s_t__builder.Start <OnConnectedAsyncd__3 > (ref d__);
            return d__.s_t__builder.Task;
        }

        [AsyncStateMachine((typeof(OnDisconnectedAsyncd__4)))]
        public override Task OnDisconnectedAsync(Exception exception)
        {
           OnDisconnectedAsyncd__4 d__=new OnDisconnectedAsyncd__4();
            d__.s_4__this = this;
            d__.exception = exception;
            d__.s_t__builder = AsyncTaskMethodBuilder.Create();
            d__.s_1__state = -1;
            d__.s_t__builder.Start <OnDisconnectedAsyncd__4 > (ref d__);
            return d__.s_t__builder.Task;
        }

        public Task SendMessage(string message, List<string> userIds, string userName = "")
        {
            return ClientProxyExtensions.SendAsync(_hubContext.Clients.Groups((IReadOnlyList<string>)userIds), "SendMessage", userName.IsNullOrWhiteSpace() ? "系统" : userName, message, new CancellationToken());
        }

        // Nested Types
        [CompilerGenerated]
        private struct OnConnectedAsyncd__3 : IAsyncStateMachine
    {
        // Fields
        public int s_1__state;
        public SignalRHub  s_4__this;
        public AsyncTaskMethodBuilder  s_t__builder;
        private TaskAwaiter  s_u__1;

        // Methods
         void IAsyncStateMachine.MoveNext()
        {
            int num = this.s_1__state;
            SignalRHub hub = this.s_4__this;
            try
            {
                TaskAwaiter awaiter;
                if (num != 0)
                {
                    if (num != 1)
                    {
                        string str = User.CurrentUserId.ToString().ToLower();
                        awaiter = hub.Groups.AddToGroupAsync(hub.Context.ConnectionId, str, new CancellationToken()).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.s_1__state = num = 0;
                            this.s_u__1 = awaiter;
                            this.s_t__builder.AwaitUnsafeOnCompleted < TaskAwaiter, SignalRHub.OnConnectedAsyncd__3 > (ref awaiter, ref this);
                            return;
                        }
                        goto Label_00A3;
                    }
                    goto Label_00E1;
                }
                awaiter = this.s_u__1;
                this.s_u__1 = new TaskAwaiter();
                this.s_1__state = num = -1;
                Label_00A3:
                awaiter.GetResult();
                awaiter = hub.s_n__0().GetAwaiter();
                if (awaiter.IsCompleted)
                {
                    goto Label_00FE;
                }
                this.s_1__state = num = 1;
                this.s_u__1 = awaiter;
                this.s_t__builder.AwaitUnsafeOnCompleted < TaskAwaiter, SignalRHub.OnConnectedAsyncd__3 > (ref awaiter, ref this);
                return;
                Label_00E1:
                awaiter = this.s_u__1;
                this.s_u__1 = new TaskAwaiter();
                this.s_1__state = num = -1;
                Label_00FE:
                awaiter.GetResult();
            }
            catch (Exception exception)
            {
                this.s_1__state = -2;
                this.s_t__builder.SetException(exception);
                return;
            }
            this.s_1__state = -2;
            this.s_t__builder.SetResult();
        }

           

            [DebuggerHidden]
         void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
            this.s_t__builder.SetStateMachine(stateMachine);
        }

           
        }

    [CompilerGenerated]
    private struct OnDisconnectedAsyncd__4 : IAsyncStateMachine
    {
        // Fields
        public int s_1__state;
        public SignalRHub  s_4__this;
        public AsyncTaskMethodBuilder   s_t__builder;
    private TaskAwaiter   s_u__1;
    public Exception exception;

    // Methods
     void IAsyncStateMachine.MoveNext()
    {
        int num = this.s_1__state;
        SignalRHub hub = this.s_4__this;
        try
        {
            TaskAwaiter awaiter;
            if (num != 0)
            {
                if (num != 1)
                {
                    string str = User.CurrentUserId.ToString().ToLower();
                    awaiter = hub.Groups.RemoveFromGroupAsync(hub.Context.ConnectionId, str, new CancellationToken()).GetAwaiter();
                    if (!awaiter.IsCompleted)
                    {
                        this.s_1__state = num = 0;
                        this.s_u__1 = awaiter;
                        this.s_t__builder.AwaitUnsafeOnCompleted < TaskAwaiter, SignalRHub.OnDisconnectedAsyncd__4 > (ref awaiter, ref this);
                        return;
                    }
                    goto Label_00A3;
                }
                goto Label_00E7;
            }
            awaiter = this.s_u__1;
            this.s_u__1 = new TaskAwaiter();
            this.s_1__state = num = -1;
            Label_00A3:
            awaiter.GetResult();
            awaiter = hub.s_n__1(this.exception).GetAwaiter();
            if (awaiter.IsCompleted)
            {
                goto Label_0104;
            }
            this.s_1__state = num = 1;
            this.s_u__1 = awaiter;
            this.s_t__builder.AwaitUnsafeOnCompleted < TaskAwaiter, SignalRHub.OnDisconnectedAsyncd__4 > (ref awaiter, ref this);
            return;
            Label_00E7:
            awaiter = this.s_u__1;
            this.s_u__1 = new TaskAwaiter();
            this.s_1__state = num = -1;
            Label_0104:
            awaiter.GetResult();
        }
        catch (Exception exception)
        {
            this.s_1__state = -2;
            this.s_t__builder.SetException(exception);
            return;
        }
        this.s_1__state = -2;
        this.s_t__builder.SetResult();
    }

    [DebuggerHidden]
     void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
    {
        this.s_t__builder.SetStateMachine(stateMachine);
    }
}
}

 

 

}
