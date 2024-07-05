using System.Collections.Generic;
using Soucore.HealthCheck.Kafka;
using Xunit;
using Xunit.Priority;

namespace Soucore.HealthCheck.Test
{
    public class HealthCheckKafka
    {
        [Theory]
        [InlineData("CONNECTED", true)]
        [InlineData("ERROR", false)]
        [InlineData("FAIL", false)]
        [InlineData("DESCONECTED", false)]
        public void Facility_Log_Status_result(string facility, bool result)
        {
            // Act
            var value = facility.IsConnected();

            //Assert
            Assert.True((value == 1) == result);
        }

        
        [Theory]
        [InlineData("CONNECTED", true)]
        [InlineData("ERROR", false)]
        [InlineData("FAIL", false)]
        public void HealthCheckFacade_Log_Facility(string facility, bool result)
        {
            // Act
            KafkaHealthCheckLogMonitor.InvokeFacilityLog(facility);
            var value = KafkaHealthCheckLogMonitor.Status();
            //Assert
            Assert.True(value == result);
        }

        [Theory]
        [InlineData("DESCONECTED")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("ABCD")]
        public void HealthCheckFacade_Log_Facility_NotEnum_Enumrable(string facility)
        {
            //A 
            KafkaHealthCheckLogMonitor.InvokeFacilityLog("ERROR");

            // Act
            KafkaHealthCheckLogMonitor.InvokeFacilityLog(facility);
            var value = KafkaHealthCheckLogMonitor.Status();

            //Assert
            Assert.True(value == false);
        }

        [Fact]
        public void HealthCheckFacade_Maintains_Last_Valid_State_True()
        {
            //A
            bool status = false;


            //Act
            KafkaHealthCheckLogMonitor.InvokeFacilityLog("CONNECTED");
            if(KafkaHealthCheckLogMonitor.Status())
            {
                KafkaHealthCheckLogMonitor.InvokeFacilityLog("ABCD");
                status = KafkaHealthCheckLogMonitor.Status();
            }

            //Assert
            Assert.True(status);
        }

        [Theory]
        [InlineData("FAIL")]
        [InlineData("ERROR")]
        public void HealthCheckFacade_Maintains_Last_Valid_State_False(string facility)
        {
            //A
            bool status = true;

            //Act
            KafkaHealthCheckLogMonitor.InvokeFacilityLog(facility);
            if (!KafkaHealthCheckLogMonitor.Status())
            {
                KafkaHealthCheckLogMonitor.InvokeFacilityLog("ABCD");
                status = KafkaHealthCheckLogMonitor.Status();
            }

            //Assert
            Assert.False(status);
        }
    }
}
