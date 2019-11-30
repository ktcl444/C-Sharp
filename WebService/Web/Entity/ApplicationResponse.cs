namespace Web.Entity
{
    public class ApplicationResponse
    {
        /// <summary>
        /// 正常执行时的返回信息
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// 是否包含返回值
        /// </summary>
        public bool HasReturnValue { get; set; }
    }
}
