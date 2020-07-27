using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvmApp.Helpers;

namespace WpfMvvmApp.Models
{
    public class Person
    {
        // 기본 변수 -> table상 필드
        public string FirstName { get; set; } // table상 필드
        public string LastName { get; set; } // table상 필드
        string email; // table상 필드
        public string Email
        {
            get => email; // { return eamil; }
            set
            {
                if (Commons.IsValidEmail(value))
                    email = value;
                else
                    throw new Exception("Invalid email");
            }
        }
        DateTime date; // table상 필드
        public DateTime Date
        {
            get => date;
            set
            {
                var result = Commons.CalcAge(value);
                if (result > 150 || result < 0)
                    throw new Exception("Invalid date");
                else
                    date = value;
            }
        }

        public string Zodiac { get { return Commons.CalcZodiac(date); } }
        public string ChnZodiac { get { return Commons.CalcChnZodiac(date); } }

        /// <summary>
        /// Person 생성자
        /// </summary>
        /// <param name="firstName">이름</param>
        /// <param name="lastName">성</param>
        /// <param name="email">이메일</param>
        /// <param name="date">생년월일</param>
        public Person(string firstName, string lastName, string email, DateTime date)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Date = date;
        }

        /// <summary>
        /// 생일 확인
        /// </summary>
        public bool IsBirthday // 추가 속성
        {
            get
            {
                return (DateTime.Now.Month == Date.Month &&
                       DateTime.Now.Day == Date.Day);
            }
        }
        
        /// <summary>
        /// 성인 여부 확인
        /// </summary>
        public bool IsAdult // 추가 속성
        {
            get
            {
                return Commons.CalcAge(date) > 18;
            }
        }
    }
}
