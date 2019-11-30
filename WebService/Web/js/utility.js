function InvokeService1(sServiceName, sOperationName, oParameters, callback, async) {
    /// <summary>调用请求访问入口，执行App服务方法（oParameters数组只允许传递字符串）</summary>
    /// <param name="sServiceName" type="String">当前App服务对应的业务单元名称。</param>
    /// <param name="sOperationName" type="String">需要执行的App服务方法名称。</param>
    /// <param name="oParameters" type="Object">向服务器发送请求的参数(数组类型)</param>
    /// <param name="callback" type="Function">执行成功后触发</param>
    /// <param name="async" type="Boolean">是否需要异步执行，默认false（即同步执行）</param>
    /// <returns type=""></returns>
    var pageParams = {};

    var ps = { serviceName: sServiceName, methodName: sOperationName, customParams: oParameters, queryString: pageParams };
    if (async == false || typeof (async) == 'undefined') {
        var data = call('Web.Proxy.ApplicationHandler', 'ProcessRequestService', ps);
        return data;
    } else
        call('Web.Proxy.ApplicationHandler', 'ProcessRequestService', ps, function(asyncData) {
            return asyncData;
        });

    return null;
}

function TTest() {
    return "ok";
}

