﻿using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.cw3.Nowy_folder;
using WebApplication1.DTOs.Requests;
using WebApplication1.DTOs.Responses;

namespace WebApplication1.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        [HttpPost]
        public IActionResult EnrollStudent(EnrollPromoteStudent request)
        {
            var st = new Student();
            st.Subject = request.Studies;
            st.Semester = "1";
            st.LastName = request.LastName;

            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18705;Integrated Security=True"))
            using (var command = new SqlCommand())
            {

                command.Connection = connection;
                connection.Open();
                var transaction = connection.BeginTransaction();


         
                try
                {
                    command.CommandText = "select IdStudies from Studies where name=@name";
                    command.Parameters.AddWithValue("name", request.Studies);

                    var dr = command.ExecuteReader();

                    if (!dr.Read())
                    {
                        transaction.Rollback();
                        return BadRequest("Studia nie istnieja.");
                    }
                    int idStudies = (int)dr["IdStudies"];
                   

                    command.CommandText = "select * from Enrollment " +
                        "where StartDate = (select MAX(StartDate) from Enrollment where idStudy=" + idStudies + ") " +
                        "AND Semester=1";

                    var execread = command.ExecuteReader();

                   .
                    if (!execread.Read())
                    {
                        command.CommandText = "Insert into Enrollment values (" +
                            "(SELECT MAX(idEnrollment)+1 from Enrollment),1," + idStudies + ",GetDate()";
                    }
                   
                    command.CommandText = "Select * from Student WHERE IndexNumber=@indexNumber";
                    command.Parameters.AddWithValue("indexNumber", request.IndexNumber);

                    var studentread = command.ExecuteReader();

                    if (!studentread.Read())
                    {
                        transaction.Rollback();
                        return BadRequest("Index jest nieunikalny.");
                    }
                    command.CommandText = "Insert into Student values(" +
                                                                       request.IndexNumber + "," +
                                                                       request.FirstName + "," +
                                                                       request.LastName + "," +
                                                                       request.BirthDate + "," +
                                                                       idStudies +
                                                                     ")";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                }

            }

            var response = new EnrollStudentResponse();
            response.StudiesName = st.Subject;
            response.Semester = int.Parse(st.Semester);
            response.LastName = st.LastName;
            return Ok(response + "  ");
        }
    }
}
