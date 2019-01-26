# BirthdayReminder

The purpose of this application is to remind about birthdays. Unfortunately I had some issues with google calendar not reminding about contacts' birthdays. This is why I decided to write this program. 

Another reason was to practice Solid principles as well as design patterns I learned about. It may sometimes be an "overkill", but the goal was to train. That way I utilized principles and patterns such as:
- Solid (single responsibilty, by creating different classes for different tasks, interface segregation by using multiple interfaces and dependency inversion with dependency injection by injecting objects through constructor with arguments specified as interface objects).
- Creational patterns such as factory and builder for creating objects
- Strategy to select contact import algorithm after selecting a file (currently it is possible to import google csv format and vcf3.0, but it is easy to extend it for different formats)
- NullObject to create objects with no actions what can be used to avoid checking for null or for unit tests.
- Bridge - for dependency injection
- Decorator - applying decorator for notifiers enables to retry notifying multiple times before throwing exception

I also used other elements:
- Data Protection API for storing password with possibility of retrieval
- MimeKit / MailKit for sending emails
- basics of WPF MVVM pattern by spliting Views with UI and ViewModels with logic
- Commands (ICommand)
- Saving to registry for autostarting application
- using NotifyIcon from WindowsForms (in WPF) in order to be able to show notifications in the system
- Settings and Resources in Properties, for storing data between runtimes
- XML for storing contacts data
- basics of LINQ
- basic parallel execution - TPL and async / await in order to import data or send emails without freezing UI.

# Conclusion

It is a simple project, but it was a great opportunity to learn a lot of small things. I know I made mistakes, such as referencing Views from ViewModels, but I have overcome many issues I encountered. During the next project it might be good idea to use MVVM framework or to use database. The model in this project is one class but it was not the main task. I tried using patterns I learned, and during the process I learn other things. This is a big plus :)

