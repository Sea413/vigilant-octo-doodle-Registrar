using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ViewEngines.Razor;

namespace Registrar
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => {
        return View["index.cshtml"];
      };

      Get["/students"] = _ => {
      List<Student> AllStudents = Student.GetAll();
      return View["students.cshtml", AllStudents];
      };

      Get["/classes"] = _ => {
      List<Class> AllClasses = Class.GetAll();
      return View["classes.cshtml", AllClasses];
      };

      Get["/class/{id}"] = parameters => {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Class selectedClass = Class.Find(parameters.id);
      List<Student> allStudents = Student.GetAll();
      List<Student> classStudents = selectedClass.GetStudents();
      model.Add("class", selectedClass);
      model.Add("allStudents", allStudents);
      model.Add("classStudents", classStudents);
      return View["class.cshtml", model];
      };

      Post["class/add_student"] = _ => {
      Class selectedClass = Class.Find(Request.Form["class-id"]);
      Student student = Student.Find(Request.Form["student-id"]);
      selectedClass.AddStudent(student);
      List<Class> AllClasses = Class.GetAll();
      return View["classes.cshtml", AllClasses];
    };

      Post["/students"] = _ => {
      Student newStudent = new Student(Request.Form["student_name"], Request.Form["student_enrollment"]);
      newStudent.Save();
      List<Student> AllStudents = Student.GetAll();
      return View["students.cshtml", AllStudents];
      };

      Post["/classes"] = _ => {
      Class newClass = new Class(Request.Form["class_name"], Request.Form["class_code"]);
      newClass.Save();
      List<Class> AllClasses = Class.GetAll();
      return View["classes.cshtml", AllClasses];
      };
    }
  }
}
