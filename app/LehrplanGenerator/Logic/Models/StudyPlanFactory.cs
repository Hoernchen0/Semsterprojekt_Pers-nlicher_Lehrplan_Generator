using System;
using System.Collections.Generic;

namespace LehrplanGenerator.Logic.Models;

public static class StudyPlanFactory
{
    public static StudyPlan CreateDemoPlan()
    {
        return new StudyPlan(
            "Lernplan",
            new List<DayPlan>
            {
                new DayPlan(
                    new DateTime(2025, 12, 25),
                    new List<TaskItem>
                    {
                        new TaskItem(
                            "Avalonia Basics",
                            "17:30",
                            "18:00",
                            "Controls, Layouts, Styles"
                        ),

                        new TaskItem(
                            "MVVM verstehen",
                            "09:00",
                            "11:00",
                            "View, ViewModel, Binding, Commands"
                        ),
                        new TaskItem(
                            "Avalonia Basics",
                            "11:30",
                            "13:00",
                            "Controls, Layouts, Styles"
                        )
                    }
                ),
                new DayPlan(
                    new DateTime(2025, 12, 26),
                    new List<TaskItem>
                    {
                        new TaskItem(
                            "TreeView",
                            "10:00",
                            "12:00",
                            "Hierarchische Daten darstellen"
                        )
                    }
                ),new DayPlan(
                    new DateTime(2025, 12, 24),
                    new List<TaskItem>
                    {
                        new TaskItem(
                            "MVVM verstehen",
                            "09:00",
                            "11:00",
                            "View, ViewModel, Binding, Commands"
                        ),
                        new TaskItem(
                            "Avalonia Basics",
                            "11:30",
                            "13:00",
                            "Controls, Layouts, Styles"
                        )
                    }
                )
            }
        );
    }
}
