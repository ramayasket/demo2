using Demo2.Core;
using Quartz;

namespace Demo2.Service
{
    public class Runner(Demonstrator demonstrator) : IJob
    {
        /// <remarks>
        /// ���������� �������������
        /// </remarks>
        public async Task Execute(IJobExecutionContext context) => await demonstrator.Invoke();
    }
}