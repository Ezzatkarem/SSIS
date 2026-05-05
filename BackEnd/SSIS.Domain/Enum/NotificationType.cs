using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Enum
{
    public enum NotificationType
    {
        FeeCreated=1,
        FeeReminder=2, //Background Job (Hangfire)
        PaymentSeccess =3,
        PaymentFaild=4,
        FeeOverDue=5,  //Background Job (Hangfire)


        UserRegisterd =10,
        ProfileUpdate=11,
        UserDelete=12,
        WelcomeMessage=13,


        courseCreated=20,
        CourseUpdated=21,
        CourseDeletred=22,


        EnrollmentCreated=30,
        EnrollmentRemoved=31,


        GradeEnterd=40,
        GradeUpdate=41,
        AttendanceRecorded=50,

        AdmonBroadcast = 100,
        DoctorBroadcast = 101
    }
}
