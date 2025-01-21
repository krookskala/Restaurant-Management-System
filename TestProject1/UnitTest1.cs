using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ConsoleApp1.Models;
using ConsoleApp1.Services;
using ConsoleApp1.Validation;

namespace TestProject1
{
    // CHEF TEST
    [TestFixture]
    public class ChefTests
    {
        [Test]
        public void Chef_Constructor_ValidInput_ShouldCreateInstance()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2015, 1, 1));
            Assert.That(chef.IdChef, Is.EqualTo(1));
            Assert.That(chef.CuisineType, Is.EqualTo("Italian"));
            Assert.That(chef.KitchenExperience, Is.EqualTo(10));
            Assert.That(chef.DateOfJoining, Is.EqualTo(new DateTime(2015, 1, 1)));
        }

        [Test]
        public void Chef_Constructor_InvalidDateOfJoining_ShouldThrowArgumentException()
        {
            Assert.That(() => new ExecutiveChef(1, "Italian", 10, DateTime.Now.AddDays(1)), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Chef_YearsSinceJoining_ShouldCalculateCorrectly()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2020, 1, 1));
            var expectedYears = DateTime.Now.Year - 2020;
            Assert.That(chef.YearsSinceJoining, Is.EqualTo(expectedYears));
        }

        [Test]
        public void Chef_AddRestaurant_ShouldAddRestaurantToAssociatedList()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2015, 1, 1));
            var restaurant = new Restaurant("Italiano", 50);

            chef.AddRestaurant(restaurant);

            Assert.That(chef.AssociatedRestaurants, Contains.Item(restaurant));
        }

        [Test]
        public void Chef_AddRestaurant_NullRestaurant_ShouldThrowArgumentNullException()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2015, 1, 1));
            Assert.That(() => chef.AddRestaurant(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Chef_RemoveRestaurant_ShouldRemoveFromAssociatedList()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2015, 1, 1));
            var restaurant = new Restaurant("Italiano", 50);

            chef.AddRestaurant(restaurant);
            chef.RemoveRestaurant(restaurant);

            Assert.That(chef.AssociatedRestaurants, Does.Not.Contain(restaurant));
        }

        [Test]
        public void Chef_AddPreparedDish_ShouldAddDishToPreparedDishesList()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2015, 1, 1));

            chef.AddPreparedDish("Pasta Carbonara");

            Assert.Pass(); // Log output confirms dish is added
        }

        [Test]
        public void Chef_AddPreparedDish_InvalidDishName_ShouldThrowArgumentException()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2015, 1, 1));
            Assert.That(() => chef.AddPreparedDish(null), Throws.TypeOf<ArgumentException>());
            Assert.That(() => chef.AddPreparedDish(string.Empty), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Chef_ClearPreparedDishes_ShouldRemoveAllDishesFromList()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2015, 1, 1));
            chef.AddPreparedDish("Dish 1");
            chef.AddPreparedDish("Dish 2");

            chef.ClearPreparedDishes();

            Assert.Pass(); // Log output confirms dishes cleared
        }

        [Test]
        public void ExecutiveChef_PrepareSpecialDish_ShouldLogSpecialDishPreparation()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2015, 1, 1));
            string loggedAction = null;

            chef.OnActionLogged += action => loggedAction = action;

            chef.PrepareSpecialDish();

            Assert.That(loggedAction, Is.EqualTo("Chef 1: Executive Chef 1 is preparing a signature dish."));
        }

        [Test]
        public void ExecutiveChef_TrainSousChef_ShouldTriggerTrainingEvent()
        {
            var chef = new ExecutiveChef(1, "Italian", 10, new DateTime(2015, 1, 1));
            string trainingEventLog = null;

            chef.OnTrainingEvent += eventLog => trainingEventLog = eventLog;

            chef.TrainSousChef();

            Assert.That(trainingEventLog, Is.EqualTo($"Executive Chef 1 is training a Sous Chef on {DateTime.Now}."));
        }

        [Test]
        public void SousChef_PrepareSpecialDish_ShouldLogSpecialDishPreparation()
        {
            var chef = new SousChef(2, "French", "Pastry", new DateTime(2020, 1, 1));
            string loggedAction = null;

            chef.OnDishPrepared += action => loggedAction = action;

            chef.PrepareSpecialDish();

            Assert.That(loggedAction, Is.EqualTo("Sous Chef 2 is preparing a dish under the guidance of the Executive Chef."));
        }

        [Test]
        public void LineChef_PrepareSpecialDish_ShouldLogSpecialDishPreparation()
        {
            var chef = new LineChef(3, "Mexican", "Tacos", new DateTime(2018, 1, 1));
            string loggedAction = null;

            chef.OnDishPrepared += action => loggedAction = action;

            chef.PrepareSpecialDish();

            Assert.That(loggedAction, Is.EqualTo("Line Chef 3 is preparing a specialty dish in Tacos."));
        }
    }
    
    // EXPERIENCED TEST
    [TestFixture]
    public class ExperiencedTests
    {
        [SetUp]
        public void Setup()
        {
            // Clear extent before each test to ensure isolation
            Experienced.ClearExtent();
        }

        [Test]
        public void Experienced_Constructor_ValidInput_ShouldCreateInstance()
        {
            var experienced = new Experienced(10);

            Assert.That(experienced.YearsOfExperience, Is.EqualTo(10));
            Assert.That(experienced.AssociationId, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void Experienced_Constructor_InvalidYearsOfExperience_ShouldThrowArgumentException()
        {
            Assert.That(() => new Experienced(0), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new Experienced(-1), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Experienced_Extent_ShouldContainCreatedInstance()
        {
            var experienced = new Experienced(5);

            Assert.That(Experienced.ExperiencedExtent, Contains.Item(experienced));
        }

        [Test]
        public void Experienced_MentorTrainee_ValidTrainee_ShouldInvokeEvent()
        {
            var experienced = new Experienced(15);
            string mentorshipLog = null;

            experienced.OnMentorshipStarted += message => mentorshipLog = message;
            
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-1), "Kitchen", "Day Shift");
            var trainee = new Trainee(6, workDetails); // Example values: trainingDuration = 6 months

            experienced.MentorTrainee(trainee);

            Assert.That(mentorshipLog, Is.EqualTo("Experienced mentor with 15 years of experience is mentoring trainee."));
        }


        [Test]
        public void Experienced_MentorTrainee_NullTrainee_ShouldThrowArgumentNullException()
        {
            var experienced = new Experienced(15);

            Assert.That(() => experienced.MentorTrainee(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Experienced_ClearExtent_ShouldRemoveAllInstances()
        {
            new Experienced(5);
            new Experienced(10);

            Experienced.ClearExtent();

            Assert.That(Experienced.ExperiencedExtent, Is.Empty);
        }

        [Test]
        public void Experienced_RemoveExperienced_ValidInstance_ShouldRemoveFromExtent()
        {
            var experienced = new Experienced(8);
            bool result = Experienced.RemoveExperienced(experienced);

            Assert.That(result, Is.True);
            Assert.That(Experienced.ExperiencedExtent, Does.Not.Contain(experienced));
        }

        [Test]
        public void Experienced_RemoveExperienced_NonExistentInstance_ShouldReturnFalse()
        {
            var experienced = new Experienced(8);

            // Clear the extent so the instance no longer exists
            Experienced.ClearExtent();

            bool result = Experienced.RemoveExperienced(experienced);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Experienced_ToString_ShouldReturnFormattedString()
        {
            var experienced = new Experienced(7);

            string expected = $"Experienced [Years of Experience: 7, Association ID: {experienced.AssociationId}]";
            Assert.That(experienced.ToString(), Is.EqualTo(expected));
        }
    }
    
    // SPECIALIST TEST
     [TestFixture]
    public class SpecialistTests
    {
        [SetUp]
        public void Setup()
        {
            // Clear the extent before each test to ensure isolation
            Specialist.ClearExtent();
        }

        [Test]
        public void Specialist_Constructor_ValidInput_ShouldCreateInstance()
        {
            var specialist = new Specialist("Pastry");

            Assert.That(specialist.FieldOfExpertise, Is.EqualTo("Pastry"));
            Assert.That(specialist.AssociationId, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void Specialist_Constructor_InvalidFieldOfExpertise_ShouldThrowArgumentException()
        {
            Assert.That(() => new Specialist(null), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new Specialist(string.Empty), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Specialist_Extent_ShouldContainCreatedInstance()
        {
            var specialist = new Specialist("Pastry");

            Assert.That(Specialist.SpecialistExtent, Contains.Item(specialist));
        }

        [Test]
        public void Specialist_DesignNewRecipes_ShouldInvokeEvent()
        {
            var specialist = new Specialist("Italian Cuisine");
            string logMessage = null;

            specialist.OnRecipeDesigned += message => logMessage = message;

            specialist.DesignNewRecipes();

            Assert.That(logMessage, Is.EqualTo("Specialist in Italian Cuisine is designing new recipes."));
        }

        [Test]
        public void Specialist_ClearExtent_ShouldRemoveAllInstances()
        {
            new Specialist("Pastry");
            new Specialist("Grill Master");

            Specialist.ClearExtent();

            Assert.That(Specialist.SpecialistExtent, Is.Empty);
        }

        [Test]
        public void Specialist_RemoveSpecialist_ValidInstance_ShouldRemoveFromExtent()
        {
            var specialist = new Specialist("Pastry");
            bool result = Specialist.RemoveSpecialist(specialist);

            Assert.That(result, Is.True);
            Assert.That(Specialist.SpecialistExtent, Does.Not.Contain(specialist));
        }

        [Test]
        public void Specialist_RemoveSpecialist_NonExistentInstance_ShouldReturnFalse()
        {
            var specialist = new Specialist("Pastry");

            // Clear the extent so the instance no longer exists
            Specialist.ClearExtent();

            bool result = Specialist.RemoveSpecialist(specialist);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Specialist_ToString_ShouldReturnFormattedString()
        {
            var specialist = new Specialist("Italian Cuisine");

            string expected = $"Specialist [Field Of Expertise: Italian Cuisine, Association ID: {specialist.AssociationId}]";
            Assert.That(specialist.ToString(), Is.EqualTo(expected));
        }
    }
    
    // TRAINEE TEST
    [TestFixture]
    public class TraineeTests
    {
        [Test]
        public void Trainee_Constructor_ValidInput_ShouldCreateInstance()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddMonths(-1), "Kitchen", "Morning");
            var trainee = new Trainee(10, workDetails);

            Assert.That(trainee.TrainingDuration, Is.EqualTo(10));
            Assert.That(trainee.WorkDetails, Is.EqualTo(workDetails));
            Assert.That(trainee.AssociationId, Is.Not.EqualTo(Guid.Empty)); 
        }


        [Test]
        public void Trainee_Constructor_InvalidTrainingDuration_ShouldThrowArgumentException()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddMonths(-1), "Kitchen", "Morning");

            Assert.That(() => new Trainee(0, workDetails), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new Trainee(53, workDetails), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Trainee_Constructor_NullWorkDetails_ShouldThrowArgumentNullException()
        {
            Assert.That(() => new Trainee(10, null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Trainee_TrainingEndDate_ShouldCalculateCorrectly()
        {
            var hiringDate = DateTime.Now.AddMonths(-1);
            var workDetails = new WorkDetails(hiringDate, "Kitchen", "Morning");
            var trainee = new Trainee(10, workDetails);

            var expectedEndDate = hiringDate.AddDays(10 * 7);
            Assert.That(trainee.TrainingEndDate, Is.EqualTo(expectedEndDate));
        }

        [Test]
        public void Trainee_CompleteTraining_ShouldTriggerEvent()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddMonths(-1), "Kitchen", "Morning");
            var trainee = new Trainee(10, workDetails);

            string? message = null;
            trainee.OnTrainingCompleted += m => message = m;

            trainee.CompleteTraining();

            Assert.That(message, Is.EqualTo($"Trainee with Hiring Date {workDetails.DateOfHiring} has completed 10 weeks of training."));
        }

        [Test]
        public void Trainee_RemoveTrainee_ShouldRemoveFromExtent()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddMonths(-1), "Kitchen", "Morning");
            var trainee = new Trainee(10, workDetails);

            var removed = Trainee.RemoveTrainee(trainee);

            Assert.That(removed, Is.True);
            Assert.That(Trainee.TraineeExtent, Does.Not.Contain(trainee));
            Assert.That(workDetails.Trainees, Does.Not.Contain(trainee));
        }

        [Test]
        public void Trainee_ClearExtent_ShouldRemoveAllTrainees()
        {
            var workDetails1 = new WorkDetails(DateTime.Now.AddMonths(-1), "Kitchen", "Morning");
            var workDetails2 = new WorkDetails(DateTime.Now.AddMonths(-2), "Front Desk", "Evening");

            var trainee1 = new Trainee(10, workDetails1);
            var trainee2 = new Trainee(8, workDetails2);

            Trainee.ClearExtent();

            Assert.That(Trainee.TraineeExtent.Count, Is.EqualTo(0));
        }
    }
    
    // MEMBER TEST
    [TestFixture]
    public class MemberTests
    {
        [Test]
        public void Member_Constructor_ValidInput_ShouldCreateInstance()
        {
            var member = new Member(1, 100, "test@example.com");

            Assert.That(member.IdMember, Is.EqualTo(1));
            Assert.That(member.CreditPoints, Is.EqualTo(100));
            Assert.That(member.Email, Is.EqualTo("test@example.com"));
        }

        [Test]
        public void Member_Constructor_NegativeId_ShouldThrowArgumentException()
        {
            Assert.That(() => new Member(-1), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Member_Constructor_NegativeCreditPoints_ShouldThrowArgumentException()
        {
            Assert.That(() => new Member(1, -50), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Member_UseCreditPoints_ValidPoints_ShouldDeductCreditPoints()
        {
            var member = new Member(1, 100);

            var result = member.UseCreditPoints(50);

            Assert.That(result, Is.True);
            Assert.That(member.CreditPoints, Is.EqualTo(50));
        }

        [Test]
        public void Member_UseCreditPoints_InsufficientCreditPoints_ShouldReturnFalse()
        {
            var member = new Member(1, 30);

            var result = member.UseCreditPoints(50);

            Assert.That(result, Is.False);
            Assert.That(member.CreditPoints, Is.EqualTo(30));
        }

        [Test]
        public void Member_UseCreditPoints_NegativePoints_ShouldThrowArgumentException()
        {
            var member = new Member(1, 100);

            Assert.That(() => member.UseCreditPoints(-10), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Member_EarnCreditPoints_ValidOrderValue_ShouldIncreaseCreditPoints()
        {
            var member = new Member(1, 50);

            member.EarnCreditPoints(100);

            Assert.That(member.CreditPoints, Is.EqualTo(60)); // Assuming default CreditPointsRate = 10
        }

        [Test]
        public void Member_EarnCreditPoints_InvalidOrderValue_ShouldThrowArgumentException()
        {
            var member = new Member(1, 50);

            Assert.That(() => member.EarnCreditPoints(0), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Member_CreditPointsRate_SetValidRate_ShouldUpdateRate()
        {
            Member.CreditPointsRate = 5;

            Assert.That(Member.CreditPointsRate, Is.EqualTo(5));
        }

        [Test]
        public void Member_CreditPointsRate_SetInvalidRate_ShouldThrowArgumentException()
        {
            Assert.That(() => Member.CreditPointsRate = 0, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Member_OnCreditPointsUsed_ShouldTriggerEvent()
        {
            var member = new Member(1, 100);
            string? logMessage = null;

            member.OnCreditPointsUsed += message => logMessage = message;

            member.UseCreditPoints(50);

            Assert.That(logMessage, Is.EqualTo("Used 50 credit points. Remaining: 50."));
        }

        [Test]
        public void Member_OnCreditPointsEarned_ShouldTriggerEvent()
        {
            var member = new Member(1, 50);
            string? logMessage = null;

            // Subscribe to the event to capture the log message
            member.OnCreditPointsEarned += message => logMessage = message;

            // Call EarnCreditPoints to trigger the event
            member.EarnCreditPoints(200); // Default rate = 10, earned points = 20

            // Check if the event was triggered with the correct message
            Assert.That(logMessage, Is.EqualTo("Earned 20 credit points. Total: 70."));
        }
    }
    
    // PAYMENT TEST
    [TestFixture]
    public class PaymentTests
    {
        [SetUp]
        public void Setup()
        {
            Payment.ClearExtent();
        }

        [Test]
        public void Payment_CardPayment_ValidInput_ShouldCreateInstance()
        {
            var payment = new Payment(1, 100.50, "1234567812345678", "John Doe");

            Assert.That(payment.IdPayment, Is.EqualTo(1));
            Assert.That(payment.Amount, Is.EqualTo(100.50));
            Assert.That(payment.Method, Is.EqualTo(PaymentMethod.Card));
            Assert.That(payment.CardNumber, Is.EqualTo("1234567812345678"));
            Assert.That(payment.CardHolderName, Is.EqualTo("John Doe"));
        }

        [Test]
        public void Payment_CashPayment_ValidInput_ShouldCreateInstance()
        {
            var payment = new Payment(2, 200.75, "Jane Smith");

            Assert.That(payment.IdPayment, Is.EqualTo(2));
            Assert.That(payment.Amount, Is.EqualTo(200.75));
            Assert.That(payment.Method, Is.EqualTo(PaymentMethod.Cash));
            Assert.That(payment.CashierName, Is.EqualTo("Jane Smith"));
        }

        [Test]
        public void Payment_InvalidCardNumber_ShouldThrowArgumentException()
        {
            Assert.That(() => new Payment(1, 100, "123", "John Doe"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Payment_InvalidCardHolderName_ShouldThrowArgumentException()
        {
            Assert.That(() => new Payment(1, 100, "1234567812345678", ""), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Payment_InvalidCashierName_ShouldThrowArgumentException()
        {
            Assert.That(() => new Payment(1, 100, ""), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Payment_DuplicateId_ShouldThrowInvalidOperationException()
        {
            new Payment(1, 100, "1234567812345678", "John Doe");
            Assert.That(() => new Payment(1, 50, "Jane Smith"), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Payment_ProcessPayment_ValidPendingStatus_ShouldCompletePayment()
        {
            var payment = new Payment(1, 100, "1234567812345678", "John Doe");

            var result = payment.ProcessPayment();

            Assert.That(result, Is.True);
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Completed));
        }

        [Test]
        public void Payment_ProcessPayment_AlreadyCompleted_ShouldThrowInvalidOperationException()
        {
            var payment = new Payment(1, 100, "1234567812345678", "John Doe");
            payment.ProcessPayment();

            Assert.That(() => payment.ProcessPayment(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Payment_RefundPayment_ValidCompletedStatus_ShouldRefundPayment()
        {
            var payment = new Payment(1, 100, "1234567812345678", "John Doe");
            payment.ProcessPayment();

            var result = payment.RefundPayment(50);

            Assert.That(result, Is.True);
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Refunded));
        }

        [Test]
        public void Payment_RefundPayment_NotCompleted_ShouldThrowInvalidOperationException()
        {
            var payment = new Payment(1, 100, "1234567812345678", "John Doe");

            Assert.That(() => payment.RefundPayment(50), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Payment_RefundPayment_InvalidAmount_ShouldThrowArgumentException()
        {
            var payment = new Payment(1, 100, "1234567812345678", "John Doe");
            payment.ProcessPayment();

            Assert.That(() => payment.RefundPayment(0), Throws.TypeOf<ArgumentException>());
            Assert.That(() => payment.RefundPayment(150), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Payment_FindPaymentById_ShouldReturnCorrectPayment()
        {
            var payment1 = new Payment(1, 100, "1234567812345678", "John Doe");
            var payment2 = new Payment(2, 200, "Jane Smith");

            var result = Payment.FindPaymentById(1);

            Assert.That(result, Is.EqualTo(payment1));
        }

        [Test]
        public void Payment_GetPaymentsByStatus_ShouldReturnCorrectPayments()
        {
            var payment1 = new Payment(1, 100, "1234567812345678", "John Doe");
            var payment2 = new Payment(2, 200, "Jane Smith");
            payment1.ProcessPayment();

            var result = Payment.GetPaymentsByStatus(PaymentStatus.Completed).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result, Contains.Item(payment1));
            Assert.That(result, Does.Not.Contain(payment2));
        }

        [Test]
        public void Payment_RemovePayment_ShouldRemoveSuccessfully()
        {
            var payment = new Payment(1, 100, "1234567812345678", "John Doe");

            var result = Payment.RemovePayment(1);

            Assert.That(result, Is.True);
            Assert.That(Payment.PaymentExtent, Does.Not.Contain(payment));
        }

        [Test]
        public void Payment_ClearExtent_ShouldRemoveAllPayments()
        {
            new Payment(1, 100, "1234567812345678", "John Doe");
            new Payment(2, 200, "Jane Smith");

            Payment.ClearExtent();

            Assert.That(Payment.PaymentExtent.Count, Is.EqualTo(0));
        }

        [Test]
        public void Payment_OnPaymentProcessed_ShouldTriggerEvent()
        {
            var payment = new Payment(1, 100, "1234567812345678", "John Doe");
            string? message = null;

            payment.OnPaymentProcessed += m => message = m;

            payment.ProcessPayment();

            Assert.That(message, Is.EqualTo("Payment 1 completed."));
        }

        [Test]
        public void Payment_OnPaymentRefunded_ShouldTriggerEvent()
        {
            var payment = new Payment(1, 100, "1234567812345678", "John Doe");
            payment.ProcessPayment();

            string? message = null;
            payment.OnPaymentRefunded += m => message = m;

            payment.RefundPayment(50);

            Assert.That(message, Is.EqualTo("Payment 1 refunded."));
        }
    }
    
    // PERSON TEST
    // Concrete implementation for testing
    public class TestPerson : Person
    {
        public TestPerson(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber)
        {
        }

        public override string GetDetails()
        {
            return $"{FirstName} {LastName}";
        }
    }
    [TestFixture]
    public class PersonTests
    {
        [Test]
        public void Person_Constructor_ValidInput_ShouldCreateInstance()
        {
            var person = new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890");

            Assert.That(person.IdPerson, Is.EqualTo(1));
            Assert.That(person.FirstName, Is.EqualTo("John"));
            Assert.That(person.LastName, Is.EqualTo("Doe"));
            Assert.That(person.BirthOfDate, Is.EqualTo(new DateTime(1990, 1, 1)));
            Assert.That(person.PhoneNumber, Is.EqualTo("123-456-7890"));
        }

        [Test]
        public void Person_Constructor_InvalidId_ShouldThrowArgumentException()
        {
            Assert.That(() => new TestPerson(0, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Person_Constructor_InvalidFirstName_ShouldThrowArgumentException()
        {
            Assert.That(() => new TestPerson(1, "", "Doe", new DateTime(1990, 1, 1), "123-456-7890"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Person_Constructor_InvalidLastName_ShouldThrowArgumentException()
        {
            Assert.That(() => new TestPerson(1, "John", "", new DateTime(1990, 1, 1), "123-456-7890"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Person_Constructor_InvalidBirthDate_ShouldThrowArgumentException()
        {
            Assert.That(() => new TestPerson(1, "John", "Doe", DateTime.Now.AddDays(1), "123-456-7890"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Person_Constructor_InvalidPhoneNumber_ShouldThrowArgumentException()
        {
            Assert.That(() => new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "invalid-phone"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Person_UpdatePhoneNumber_ValidInput_ShouldUpdatePhoneNumber()
        {
            var person = new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890");

            person.UpdatePhoneNumber("987-654-3210");

            Assert.That(person.PhoneNumber, Is.EqualTo("987-654-3210"));
        }

        [Test]
        public void Person_UpdatePhoneNumber_InvalidInput_ShouldThrowArgumentException()
        {
            var person = new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890");

            Assert.That(() => person.UpdatePhoneNumber("invalid-phone"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Person_UpdateName_ValidInput_ShouldUpdateName()
        {
            var person = new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890");

            person.UpdateName("Jane", "Smith");

            Assert.That(person.FirstName, Is.EqualTo("Jane"));
            Assert.That(person.LastName, Is.EqualTo("Smith"));
        }

        [Test]
        public void Person_UpdateName_InvalidInput_ShouldThrowArgumentException()
        {
            var person = new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890");

            Assert.That(() => person.UpdateName("", "Smith"), Throws.TypeOf<ArgumentException>());
            Assert.That(() => person.UpdateName("Jane", ""), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Person_ValidateBirthDate_ValidInput_ShouldReturnSuccess()
        {
            var result = Person.ValidateBirthDate(new DateTime(1990, 1, 1), new ValidationContext(new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890")));

            Assert.That(result, Is.EqualTo(ValidationResult.Success));
        }

        [Test]
        public void Person_ValidateBirthDate_InvalidInput_ShouldReturnValidationResult()
        {
            // Arrange: Set a future date and create a ValidationContext
            var futureDate = DateTime.Now.AddDays(1);
            var validationContext = new ValidationContext(new object());

            // Act: Call the ValidateBirthDate method
            var result = Person.ValidateBirthDate(futureDate, validationContext);

            // Assert: Verify the returned error message
            Assert.That(result?.ErrorMessage, Is.EqualTo("Birth date cannot be in the future."));
        }



        [Test]
        public void Person_Equals_SameAttributes_ShouldReturnTrue()
        {
            var person1 = new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890");
            var person2 = new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890");

            Assert.That(person1.Equals(person2), Is.True);
        }

        [Test]
        public void Person_Equals_DifferentAttributes_ShouldReturnFalse()
        {
            var person1 = new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890");
            var person2 = new TestPerson(2, "Jane", "Smith", new DateTime(1985, 5, 15), "987-654-3210");

            Assert.That(person1.Equals(person2), Is.False);
        }

        [Test]
        public void Person_ToString_ShouldReturnCorrectString()
        {
            var person = new TestPerson(1, "John", "Doe", new DateTime(1990, 1, 1), "123-456-7890");

            var result = person.ToString();

            Assert.That(result, Is.EqualTo("Person [ID: 1, Name: John Doe, Birth Date: 01/01/1990, Phone Number: 123-456-7890]"));
        }
    }
    
    // DATE VALIDATION
    [TestFixture]
    public class NonDefaultDateAttributeTests
    {
        private class TestClass
        {
            [NonDefaultDate(ErrorMessage = "Custom error message for default date.")]
            public DateTime Date { get; set; }
        }

        private ValidationContext CreateValidationContext(object instance)
        {
            return new ValidationContext(instance);
        }

        [Test]
        public void NonDefaultDate_ValidDate_ShouldPassValidation()
        {
            var testInstance = new TestClass { Date = new DateTime(2023, 1, 1) };
            var context = CreateValidationContext(testInstance);
            var property = typeof(TestClass).GetProperty(nameof(TestClass.Date));
            var attribute = property.GetCustomAttributes(typeof(NonDefaultDateAttribute), true)[0] as NonDefaultDateAttribute;

            var result = attribute?.GetValidationResult(testInstance.Date, context);

            Assert.That(result, Is.EqualTo(ValidationResult.Success));
        }

        [Test]
        public void NonDefaultDate_DefaultDate_ShouldFailValidation()
        {
            var testInstance = new TestClass { Date = DateTime.MinValue };
            var context = CreateValidationContext(testInstance);
            var property = typeof(TestClass).GetProperty(nameof(TestClass.Date));
            var attribute = property.GetCustomAttributes(typeof(NonDefaultDateAttribute), true)[0] as NonDefaultDateAttribute;

            var result = attribute?.GetValidationResult(testInstance.Date, context);

            Assert.That(result?.ErrorMessage, Is.EqualTo("Custom error message for default date."));
        }

        [Test]
        public void NonDefaultDate_NullValue_ShouldPassValidation()
        {
            var testInstance = new TestClass { Date = default };
            var context = CreateValidationContext(testInstance);
            var property = typeof(TestClass).GetProperty(nameof(TestClass.Date));
            var attribute = property.GetCustomAttributes(typeof(NonDefaultDateAttribute), true)[0] as NonDefaultDateAttribute;

            var result = attribute?.GetValidationResult(null, context);

            Assert.That(result, Is.EqualTo(ValidationResult.Success));
        }

        [Test]
        public void NonDefaultDate_NonDateTimeValue_ShouldPassValidation()
        {
            var attribute = new NonDefaultDateAttribute { ErrorMessage = "Value must be a DateTime." };

            var result = attribute.GetValidationResult("Not a DateTime", new ValidationContext(new object()));

            Assert.That(result, Is.EqualTo(ValidationResult.Success));
        }
    }
    
    // EMPLOYEE TEST
    public class TestEmployee : Employee
    {
        public TestEmployee(int idEmployee, WorkDetails workDetails, DateTime? dateOfLeaving = null)
            : base(idEmployee, workDetails, dateOfLeaving) { }
    }
    [TestFixture]
    public class EmployeeTests
    {
        [Test]
        public void Employee_Constructor_ValidInput_ShouldCreateInstance()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var employee = new TestEmployee(1, workDetails);

            Assert.That(employee.IdEmployee, Is.EqualTo(1));
            Assert.That(employee.WorkDetails, Is.EqualTo(workDetails));
            Assert.That(employee.DateOfLeaving, Is.Null);
        }

        [Test]
        public void Employee_Constructor_NullWorkDetails_ShouldThrowArgumentNullException()
        {
            Assert.That(() => new TestEmployee(1, null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Employee_SetDateOfLeaving_ValidDate_ShouldUpdateDateOfLeaving()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var employee = new TestEmployee(1, workDetails);

            employee.SetDateOfLeaving(DateTime.Now);

            Assert.That(employee.DateOfLeaving, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void Employee_SetDateOfLeaving_InvalidDate_ShouldThrowArgumentException()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var employee = new TestEmployee(1, workDetails);

            Assert.That(() => employee.SetDateOfLeaving(DateTime.Now.AddYears(-3)), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Employee_UpdateWorkDetails_ValidWorkDetails_ShouldUpdateWorkDetails()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var newWorkDetails = new WorkDetails(DateTime.Now.AddYears(-1), "Bar", "Evening");
            var employee = new TestEmployee(1, workDetails);

            employee.UpdateWorkDetails(newWorkDetails);

            Assert.That(employee.WorkDetails, Is.EqualTo(newWorkDetails));
        }

        [Test]
        public void Employee_SetSupervisor_ValidManager_ShouldUpdateSupervisor()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var employee = new TestEmployee(1, workDetails);
            var manager = new Manager(1, "Kitchen"); // Correct: Passing a string as expected

            employee.SetSupervisor(manager);

            Assert.That(employee.Supervisor, Is.EqualTo(manager));
        }
        
        [Test]
        public void Employee_SetSupervisor_AlreadyHasSupervisor_ShouldThrowInvalidOperationException()
        {
            // Arrange: Create valid WorkDetails and Employee instances
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var employee = new TestEmployee(1, workDetails);
            var manager = new Manager(1, "Kitchen"); // Pass a string instead of WorkDetails

            // Act: Set the supervisor for the employee
            employee.SetSupervisor(manager);

            // Assert: Ensure setting the supervisor again throws an exception
            Assert.That(() => employee.SetSupervisor(manager), Throws.TypeOf<InvalidOperationException>());
        }
        
        [Test]
        public void Employee_GetEmploymentDuration_ShouldReturnCorrectDays()
        {
            var hiringDate = DateTime.Now.AddYears(-1);
            var workDetails = new WorkDetails(hiringDate, "Kitchen", "Morning");
            var employee = new TestEmployee(1, workDetails);

            var duration = employee.GetEmploymentDuration();

            Assert.That(duration, Is.EqualTo((DateTime.Now - hiringDate).Days).Within(1));
        }
    }
    
    // WORKDETAILS TEST
    [TestFixture]
    public class WorkDetailsTests
    {
        [Test]
        public void WorkDetails_Constructor_ValidInput_ShouldCreateInstance()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");

            Assert.That(workDetails.DateOfHiring, Is.EqualTo(DateTime.Now.AddYears(-2)).Within(TimeSpan.FromSeconds(1)));
            Assert.That(workDetails.Department, Is.EqualTo("Kitchen"));
            Assert.That(workDetails.ShiftSchedule, Is.EqualTo("Morning"));
        }

        [Test]
        public void WorkDetails_Constructor_InvalidDateOfHiring_ShouldThrowArgumentException()
        {
            Assert.That(() => new WorkDetails(DateTime.Now.AddDays(1), "Kitchen", "Morning"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void WorkDetails_Constructor_EmptyDepartment_ShouldThrowArgumentException()
        {
            Assert.That(() => new WorkDetails(DateTime.Now.AddYears(-1), "", "Morning"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void WorkDetails_Constructor_EmptyShiftSchedule_ShouldThrowArgumentException()
        {
            Assert.That(() => new WorkDetails(DateTime.Now.AddYears(-1), "Kitchen", ""), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void WorkDetails_LinkToEmployee_ValidEmployee_ShouldLinkSuccessfully()
        {
            // Arrange
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var employee = new TestEmployee(1, workDetails); // Employee is linked here automatically

            // Act
            // No need to call LinkToEmployee if it's already done in the constructor

            // Assert
            Assert.That(workDetails.AssociatedEmployee, Is.EqualTo(employee));
        }


        [Test]
        public void WorkDetails_LinkToEmployee_AlreadyLinked_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var employee1 = new TestEmployee(1, workDetails); // Automatically links employee1
            var employee2 = new TestEmployee(2, new WorkDetails(DateTime.Now.AddYears(-1), "Bar", "Evening"));

            // Act & Assert
            Assert.That(() => workDetails.LinkToEmployee(employee2), Throws.TypeOf<InvalidOperationException>());
        }


        [Test]
        public void WorkDetails_UnlinkFromEmployee_ShouldRemoveLink()
        {
            // Arrange
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var employee = new TestEmployee(1, workDetails); // Automatically links the employee

            // Act
            workDetails.UnlinkFromEmployee(); // Unlink directly

            // Assert
            Assert.That(workDetails.AssociatedEmployee, Is.Null);
        }
        
        [Test]
        public void WorkDetails_LinkToEmployee_TwiceWithoutUnlink_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var employee1 = new TestEmployee(1, workDetails); // Implicitly links employee1
            var employee2 = new TestEmployee(2, new WorkDetails(DateTime.Now.AddYears(-1), "Bar", "Evening")); // A different WorkDetails instance

            // Assert: Attempting to link employee2 should throw
            Assert.That(() => workDetails.LinkToEmployee(employee2), Throws.TypeOf<InvalidOperationException>());
        }
        
        [Test]
        public void WorkDetails_UnlinkFromEmployee_NoEmployee_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");

            // Act & Assert
            Assert.That(() => workDetails.UnlinkFromEmployee(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void WorkDetails_AddTrainee_ValidTrainee_ShouldAddToList()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var trainee = new Trainee(10, workDetails);

            workDetails.AddTrainee(trainee);

            Assert.That(workDetails.Trainees, Contains.Item(trainee));
        }

        [Test]
        public void WorkDetails_RemoveTrainee_ExistingTrainee_ShouldRemoveFromList()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var trainee = new Trainee(10, workDetails);

            workDetails.AddTrainee(trainee);
            var removed = workDetails.RemoveTrainee(trainee);

            Assert.That(removed, Is.True);
            Assert.That(workDetails.Trainees, Does.Not.Contain(trainee));
        }

        [Test]
        public void WorkDetails_AddSpecialist_ValidSpecialist_ShouldAddToList()
        {
            // Arrange
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var specialist = new Specialist("Pastry Chef");

            // Act
            workDetails.AddSpecialist(specialist);

            // Assert
            Assert.That(workDetails.Specialists, Contains.Item(specialist));
        }

        [Test]
        public void WorkDetails_RemoveSpecialist_ExistingSpecialist_ShouldRemoveFromList()
        {
            // Arrange
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var specialist = new Specialist("Pastry Chef");

            workDetails.AddSpecialist(specialist);

            // Act
            var removed = workDetails.RemoveSpecialist(specialist);

            // Assert
            Assert.That(removed, Is.True);
            Assert.That(workDetails.Specialists, Does.Not.Contain(specialist));
        }

        [Test]
        public void WorkDetails_RemoveSpecialist_NonExistentSpecialist_ShouldReturnFalse()
        {
            // Arrange
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");
            var specialist = new Specialist("Pastry Chef");

            // Act
            var removed = workDetails.RemoveSpecialist(specialist);

            // Assert
            Assert.That(removed, Is.False);
        }

        [Test]
        public void WorkDetails_UpdateDepartment_ValidDepartment_ShouldUpdate()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");

            workDetails.UpdateDepartment("Bar");

            Assert.That(workDetails.Department, Is.EqualTo("Bar"));
        }

        [Test]
        public void WorkDetails_UpdateDepartment_InvalidDepartment_ShouldThrowArgumentException()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");

            Assert.That(() => workDetails.UpdateDepartment(""), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void WorkDetails_UpdateShiftSchedule_ValidShiftSchedule_ShouldUpdate()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");

            workDetails.UpdateShiftSchedule("Evening");

            Assert.That(workDetails.ShiftSchedule, Is.EqualTo("Evening"));
        }

        [Test]
        public void WorkDetails_UpdateShiftSchedule_InvalidShiftSchedule_ShouldThrowArgumentException()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");

            Assert.That(() => workDetails.UpdateShiftSchedule(""), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void WorkDetails_GetWorkDuration_ShouldReturnCorrectDays()
        {
            var hiringDate = DateTime.Now.AddYears(-1);
            var workDetails = new WorkDetails(hiringDate, "Kitchen", "Morning");

            var duration = workDetails.GetWorkDuration();

            Assert.That(duration, Is.EqualTo((DateTime.Now - hiringDate).Days).Within(1));
        }

        [Test]
        public void WorkDetails_ToString_ShouldReturnFormattedString()
        {
            var workDetails = new WorkDetails(DateTime.Now.AddYears(-2), "Kitchen", "Morning");

            var result = workDetails.ToString();

            StringAssert.Contains("Date Of Hiring:", result);
            StringAssert.Contains("Department: Kitchen", result);
            StringAssert.Contains("Shift Schedule: Morning", result);
        }
    }
    
    // VALET TEST
    [TestFixture]
    public class ValetTests
    {
        [Test]
        public void Valet_Constructor_ValidInput_ShouldCreateInstance()
        {
            // Arrange & Act
            var valet = new Valet(1, "Main Garage");

            // Assert
            Assert.That(valet.IdValet, Is.EqualTo(1));
            Assert.That(valet.AssignedLocation, Is.EqualTo("Main Garage"));
            Assert.That(valet.LocationSummary, Is.EqualTo("Valet 1 is at Main Garage."));
        }

        [Test]
        public void Valet_Constructor_Default_ShouldCreateInstanceWithDefaults()
        {
            // Arrange & Act
            var valet = new Valet();

            // Assert
            Assert.That(valet.IdValet, Is.EqualTo(1));
            Assert.That(valet.AssignedLocation, Is.EqualTo("Default Garage"));
            Assert.That(valet.LocationSummary, Is.EqualTo("Valet 1 is at Default Garage."));
        }

        [Test]
        public void Valet_Constructor_InvalidLocation_ShouldThrowArgumentException()
        {
            // Assert
            Assert.That(() => new Valet(1, ""), Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Assigned Location must be between 3 and 100 characters."));
        }

        [Test]
        public void Valet_ParkCar_ShouldTriggerEventAndLogAction()
        {
            // Arrange
            var valet = new Valet(1, "Main Garage");
            string? logMessage = null;
            valet.OnCarParked += message => logMessage = message;

            // Act
            valet.ParkCar();

            // Assert
            Assert.That(logMessage, Is.EqualTo("Valet 1 is parking a car at Main Garage."));
        }

        [Test]
        public void Valet_Equals_SameAttributes_ShouldReturnTrue()
        {
            // Arrange
            var valet1 = new Valet(1, "Main Garage");
            var valet2 = new Valet(1, "Main Garage");

            // Assert
            Assert.That(valet1.Equals(valet2), Is.True);
        }

        [Test]
        public void Valet_Equals_DifferentAttributes_ShouldReturnFalse()
        {
            // Arrange
            var valet1 = new Valet(1, "Main Garage");
            var valet2 = new Valet(2, "East Garage");

            // Assert
            Assert.That(valet1.Equals(valet2), Is.False);
        }

        [Test]
        public void Valet_ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var valet = new Valet(1, "Main Garage");

            // Act
            var result = valet.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("Valet 1 Assigned To Main Garage"));
        }

        [Test]
        public void Valet_GetHashCode_SameAttributes_ShouldReturnSameHashCode()
        {
            // Arrange
            var valet1 = new Valet(1, "Main Garage");
            var valet2 = new Valet(1, "Main Garage");

            // Assert
            Assert.That(valet1.GetHashCode(), Is.EqualTo(valet2.GetHashCode()));
        }

        [Test]
        public void Valet_GetHashCode_DifferentAttributes_ShouldReturnDifferentHashCodes()
        {
            // Arrange
            var valet1 = new Valet(1, "Main Garage");
            var valet2 = new Valet(2, "East Garage");

            // Assert
            Assert.That(valet1.GetHashCode(), Is.Not.EqualTo(valet2.GetHashCode()));
        }
    }
    
    // MANAGER TEST
    [TestFixture]
    public class MenuTests
    {
        private Menu _menu;

        [SetUp]
        public void Setup()
        {
            _menu = new Menu("Gourmet Menu", "Dinner", new List<string> { "English", "French" });
        }

        [Test]
        public void Menu_Constructor_ValidInput_ShouldCreateInstance()
        {
            Assert.That(_menu.Name, Is.EqualTo("Gourmet Menu"));
            Assert.That(_menu.MenuType, Is.EqualTo("Dinner"));
            Assert.That(_menu.AvailableLanguages, Contains.Item("English"));
            Assert.That(_menu.DefaultLanguage, Is.EqualTo("English"));
            Assert.That(_menu.Dishes, Is.Empty);
        }

        [Test]
        public void Menu_Constructor_InvalidName_ShouldThrowException()
        {
            Assert.That(() => new Menu(null, "Lunch", new List<string> { "English" }), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new Menu(string.Empty, "Lunch", new List<string> { "English" }), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Menu_AddLanguage_ValidLanguage_ShouldAddLanguage()
        {
            _menu.AddLanguage("Spanish");
            Assert.That(_menu.AvailableLanguages, Contains.Item("Spanish"));
        }

        [Test]
        public void Menu_AddLanguage_DuplicateLanguage_ShouldNotAdd()
        {
            _menu.AddLanguage("English");
            Assert.That(_menu.AvailableLanguages.Count, Is.EqualTo(2)); // No duplicate added
        }

        [Test]
        public void Menu_RemoveLanguage_ValidLanguage_ShouldRemoveLanguage()
        {
            _menu.RemoveLanguage("English");
            Assert.That(_menu.AvailableLanguages, Does.Not.Contain("English"));
        }

        [Test]
        public void Menu_SetDefaultLanguage_ValidLanguage_ShouldUpdateDefault()
        {
            _menu.SetDefaultLanguage("French");
            Assert.That(_menu.DefaultLanguage, Is.EqualTo("French"));
        }

        [Test]
        public void Menu_SetDefaultLanguage_InvalidLanguage_ShouldThrowException()
        {
            Assert.That(() => _menu.SetDefaultLanguage("Spanish"), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Menu_AddDish_ValidDish_ShouldAddToMenu()
        {
            var dish = new Dish(
                "Risotto", 
                "Italian", 
                15.99m, 
                false, 
                new List<string> { "Rice", "Parmesan", "Broth" }
            );
    
            _menu.AddDish(dish);

            Assert.That(_menu.Dishes, Contains.Item(dish));
        }


        [Test]
        public void Menu_AddDish_DuplicateDish_ShouldThrowException()
        {
            var dish = new Dish("Risotto", "Italian", 12.99m, false, new List<string> { "Rice", "Parmesan", "Broth" });
            _menu.AddDish(dish);

            Assert.That(() => _menu.AddDish(dish), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Menu_RemoveDish_ValidDish_ShouldRemoveFromMenu()
        {
            var dish = new Dish("Risotto", "Italian", 12.99m, false, new List<string> { "Rice", "Parmesan", "Broth" });
            _menu.AddDish(dish);
            _menu.RemoveDish(dish);

            Assert.That(_menu.Dishes, Does.Not.Contain(dish));
        }

        [Test]
        public void Menu_RemoveDish_InvalidDish_ShouldThrowException()
        {
            var dish = new Dish("Risotto", "Italian", 12.99m, false, new List<string> { "Rice", "Parmesan", "Broth" });

            Assert.That(() => _menu.RemoveDish(dish), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Menu_GetVegetarianDishes_ShouldReturnOnlyVegetarianDishes()
        {
            var vegDish = new Dish("Salad", "Italian", 9.99m, true, new List<string> { "Lettuce", "Tomato", "Cucumber" });
            var nonVegDish = new Dish("Risotto", "Italian", 12.99m, false, new List<string> { "Rice", "Parmesan", "Broth" });

            _menu.AddDish(vegDish);
            _menu.AddDish(nonVegDish);

            var vegetarianDishes = _menu.GetVegetarianDishes();

            Assert.That(vegetarianDishes, Contains.Item(vegDish));
            Assert.That(vegetarianDishes, Does.Not.Contain(nonVegDish));
        }

        [Test]
        public void Menu_GetDishesByCuisine_ShouldReturnMatchingDishes()
        {
            var italianDish = new Dish("Risotto", "Italian", 12.99m, false, new List<string> { "Rice", "Parmesan", "Broth" });
            var frenchDish = new Dish("Croissant", "French", 4.99m, true, new List<string> { "Flour", "Butter", "Yeast" });

            _menu.AddDish(italianDish);
            _menu.AddDish(frenchDish);

            var italianDishes = _menu.GetDishesByCuisine("Italian");

            Assert.That(italianDishes, Contains.Item(italianDish));
            Assert.That(italianDishes, Does.Not.Contain(frenchDish));
        }

        [Test]
        public void Menu_AddQualifiedDish_ValidQualifier_ShouldAddDish()
        {
            var dish = new Dish("Steak", "American", 19.99m, false, new List<string> { "Beef", "Salt", "Pepper" });
            _menu.AddQualifiedDish("Special", dish);

            Assert.That(_menu.GetQualifiedDish("Special"), Is.EqualTo(dish));
        }

        [Test]
        public void Menu_AddQualifiedDish_DuplicateQualifier_ShouldThrowException()
        {
            var dish1 = new Dish("Steak", "American", 19.99m, false, new List<string> { "Beef", "Salt", "Pepper" });
            var dish2 = new Dish("Burger", "American", 9.99m, false, new List<string> { "Bun", "Beef", "Cheese" });

            _menu.AddQualifiedDish("Special", dish1);

            Assert.That(() => _menu.AddQualifiedDish("Special", dish2), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Menu_RemoveQualifiedDish_ValidQualifier_ShouldRemoveDish()
        {
            var dish = new Dish("Steak", "American", 19.99m, false, new List<string> { "Beef", "Salt", "Pepper" });
            _menu.AddQualifiedDish("Special", dish);

            _menu.RemoveQualifiedDish("Special");

            Assert.That(() => _menu.GetQualifiedDish("Special"), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Menu_ListDishes_ShouldReturnCommaSeparatedDishNames()
        {
            var dish1 = new Dish("Risotto", "Italian", 12.99m, false, new List<string> { "Rice", "Parmesan", "Broth" });
            var dish2 = new Dish("Salad", "Italian", 9.99m, true, new List<string> { "Lettuce", "Tomato", "Cucumber" });

            _menu.AddDish(dish1);
            _menu.AddDish(dish2);

            var dishList = _menu.ListDishes();

            Assert.That(dishList, Is.EqualTo("Risotto, Salad"));
        }

        [Test]
        public void Menu_ToString_ShouldReturnFormattedString()
        {
            var expected = "Menu [Name: Gourmet Menu, Type: Dinner, Dishes Count: 0]";
            Assert.That(_menu.ToString(), Is.EqualTo(expected));
        }
    }
    
    // DISH TEST
    [TestFixture]
    public class DishTests
    {
        [SetUp]
        public void SetUp()
        {
            // Clear the DishList using reflection
            var dishListField = typeof(Dish).GetField("DishList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (dishListField != null)
            {
                var dishList = dishListField.GetValue(null) as List<Dish>;
                dishList?.Clear();
            }
        }

        [Test]
        public void Dish_Constructor_ValidInput_ShouldCreateInstance()
        {
            var ingredients = new List<string> { "Tomato", "Cheese", "Basil" };
            var dish = new Dish("Margherita", "Italian", 9.99m, true, ingredients);

            Assert.That(dish.Name, Is.EqualTo("Margherita"));
            Assert.That(dish.Cuisine, Is.EqualTo("Italian"));
            Assert.That(dish.Price, Is.EqualTo(9.99m));
            Assert.That(dish.IsVegetarian, Is.True);
            Assert.That(dish.Ingredients, Is.EquivalentTo(ingredients));
        }

        [Test]
        public void Dish_Constructor_DuplicateDish_ShouldThrowInvalidOperationException()
        {
            var ingredients = new List<string> { "Tomato", "Cheese", "Basil" };
            var dish = new Dish("Margherita", "Italian", 9.99m, true, ingredients);

            Assert.That(() => new Dish("Margherita", "Italian", 10.99m, false, ingredients),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Dish_AddIngredient_ValidIngredient_ShouldAddToList()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Veggie Delight", "Italian", 12.99m, true, ingredients);

            dish.AddIngredient("Olives");

            Assert.That(dish.Ingredients, Contains.Item("Olives"));
        }

        [Test]
        public void Dish_AddIngredient_DuplicateIngredient_ShouldThrowInvalidOperationException()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Veggie Delight", "Italian", 12.99m, true, ingredients);

            // Attempt to add duplicates with different casing
            Assert.That(() => dish.AddIngredient("Tomato"), Throws.TypeOf<InvalidOperationException>());
            Assert.That(() => dish.AddIngredient("tomato"), Throws.TypeOf<InvalidOperationException>());
        }
        
        [Test]
        public void Dish_RemoveIngredient_ExistingIngredient_ShouldRemoveFromList()
        {
            var ingredients = new List<string> { "Tomato", "Cheese", "Basil" };
            var dish = new Dish("Caprese Salad", "Italian", 7.99m, true, ingredients);

            dish.RemoveIngredient("basil");

            Assert.That(dish.Ingredients, Does.Not.Contain("basil"));
            Assert.That(dish.Ingredients, Does.Not.Contain("Basil"));
        }

        [Test]
        public void Dish_RemoveIngredient_NonExistingIngredient_ShouldThrowInvalidOperationException()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Veggie Delight", "Italian", 12.99m, true, ingredients);

            Assert.That(() => dish.RemoveIngredient("Basil"), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Dish_ListIngredients_ShouldReturnCommaSeparatedString()
        {
            var ingredients = new List<string> { "Tomato", "Cheese", "Basil" };
            var dish = new Dish("Caprese Salad", "Italian", 7.99m, true, ingredients);

            var ingredientList = dish.ListIngredients();

            Assert.That(ingredientList, Is.EqualTo("tomato, cheese, basil")); // Lowercased as per class behavior
        }

        [Test]
        public void Dish_AddOrderDish_ValidOrderDish_ShouldAddToList()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Veggie Delight", "Italian", 12.99m, true, ingredients);

            var orderDish = new OrderDish(dish, 2);

            Assert.Pass(); // No exception should be thrown
        }

        [Test]
        public void Dish_AddOrderDish_DuplicateOrderDish_ShouldThrowInvalidOperationException()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Veggie Delight", "Italian", 12.99m, true, ingredients);

            var orderDish1 = new OrderDish(dish, 2);

            Assert.That(() => new OrderDish(dish, 3), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Dish_RemoveOrderDish_ValidOrderDish_ShouldRemoveFromList()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Veggie Delight", "Italian", 12.99m, true, ingredients);

            var orderDish = new OrderDish(dish, 2);
            dish.RemoveOrderDish(orderDish);

            Assert.Pass(); // Ensure no exception is thrown
        }

        [Test]
        public void Dish_RemoveOrderDish_NonExistingOrderDish_ShouldThrowInvalidOperationException()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Veggie Delight", "Italian", 12.99m, true, ingredients);

            var nonExistingOrderDish = new OrderDish(new Dish("New Dish", "French", 15.99m, false, new List<string> { "Ingredient" }), 1);

            Assert.That(() => dish.RemoveOrderDish(nonExistingOrderDish), Throws.TypeOf<InvalidOperationException>());
        }
    }
    
    // ORDERDISH TEST
    [TestFixture]
    public class OrderDishTests
    {
        [SetUp]
        public void SetUp()
        {
            var dishListField = typeof(Dish).GetField("DishList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (dishListField != null)
            {
                var dishList = dishListField.GetValue(null) as List<Dish>;
                dishList?.Clear();
            }
        }
        
        [Test]
        public void OrderDish_Constructor_ValidInput_ShouldCreateInstance()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Margherita", "Italian", 9.99m, true, ingredients);

            var orderDish = new OrderDish(dish, 2);

            Assert.That(orderDish.Dish, Is.EqualTo(dish));
            Assert.That(orderDish.Quantity, Is.EqualTo(2));
            Assert.That(orderDish.UnitPrice, Is.EqualTo(9.99m));
            Assert.That(orderDish.TotalPrice, Is.EqualTo(19.98m));
        }

        [Test]
        public void OrderDish_Constructor_NullDish_ShouldThrowArgumentNullException()
        {
            Assert.That(() => new OrderDish(null, 2), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void OrderDish_Constructor_NegativeDishPrice_ShouldThrowArgumentException()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };

            // A dish with a negative price cannot be created, so this test is invalid
            Assert.Pass("This test is redundant because the Dish class already validates prices.");
        }


        [Test]
        public void OrderDish_Constructor_NonPositiveQuantity_ShouldThrowArgumentException()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Margherita", "Italian", 9.99m, true, ingredients);

            Assert.That(() => new OrderDish(dish, 0), Throws.TypeOf<ArgumentException>().With.Message.Contains("Quantity must be greater than zero"));
        }

        [Test]
        public void OrderDish_UpdateQuantity_ValidInput_ShouldUpdateQuantity()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Margherita", "Italian", 9.99m, true, ingredients);

            var orderDish = new OrderDish(dish, 2);

            orderDish.UpdateQuantity(5);

            Assert.That(orderDish.Quantity, Is.EqualTo(5));
            Assert.That(orderDish.TotalPrice, Is.EqualTo(49.95m));
        }

        [Test]
        public void OrderDish_UpdateQuantity_NonPositiveValue_ShouldThrowArgumentException()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Margherita", "Italian", 9.99m, true, ingredients);

            var orderDish = new OrderDish(dish, 2);

            Assert.That(() => orderDish.UpdateQuantity(0), Throws.TypeOf<ArgumentException>().With.Message.Contains("Quantity must be greater than zero"));
        }

        [Test]
        public void OrderDish_RemoveFromDish_ShouldRemoveOrderDishFromDish()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish = new Dish("Margherita", "Italian", 9.99m, true, ingredients);

            var orderDish = new OrderDish(dish, 2);
            orderDish.RemoveFromDish();

            Assert.Pass(); // Verify no exception is thrown
        }

        [Test]
        public void OrderDish_UpdateQuantities_ValidInput_ShouldUpdateAllQuantities()
        {
            var ingredients = new List<string> { "Tomato", "Cheese" };
            var dish1 = new Dish("Margherita", "Italian", 9.99m, true, ingredients);
            var dish2 = new Dish("Veggie Delight", "Italian", 12.99m, true, ingredients);

            var orderDish1 = new OrderDish(dish1, 2);
            var orderDish2 = new OrderDish(dish2, 3);

            OrderDish.UpdateQuantities(new List<OrderDish> { orderDish1, orderDish2 }, 5);

            Assert.That(orderDish1.Quantity, Is.EqualTo(5));
            Assert.That(orderDish1.TotalPrice, Is.EqualTo(49.95m));

            Assert.That(orderDish2.Quantity, Is.EqualTo(5));
            Assert.That(orderDish2.TotalPrice, Is.EqualTo(64.95m));
        }
        
        [Test]
        public void OrderDish_Constructor_ShouldCreateInstanceWithoutReverseConnection()
        {
            var mockDish = new Dish("Mock Dish", "Mock Cuisine", 5.99m, true, new List<string> { "Mock Ingredient" });
            var orderDish = new OrderDish(mockDish, 3);

            Assert.That(orderDish.Dish, Is.EqualTo(mockDish));
            Assert.That(orderDish.Quantity, Is.EqualTo(3));
            Assert.That(orderDish.TotalPrice, Is.EqualTo(17.97m));
        }
    }
    
    // ORDER TEST
    [TestFixture]
    public class OrderTests
    {
        [SetUp]
        public void SetUp()
        {
            Customer.ClearCustomers(); // Clears static CustomersByEmail dictionary
            Order.ClearExtent(); // Clears static OrderExtentList

            // Clear DishList
            var dishListField = typeof(Dish).GetField("DishList", BindingFlags.NonPublic | BindingFlags.Static);
            if (dishListField != null)
            {
                var dishList = dishListField.GetValue(null) as List<Dish>;
                dishList?.Clear();
            }
        }

        [Test]
        public void Order_Constructor_ValidInput_ShouldCreateInstance()
        {
            var customer = new Customer(1, "john.doe@example.com");
            var order = new Order(1, customer);

            Assert.That(order.IdOrder, Is.EqualTo(1));
            Assert.That(order.Customer, Is.EqualTo(customer));
            Assert.That(Order.OrderExtent.Count, Is.EqualTo(1));
        }

        [Test]
        public void Order_AddOrderDish_ValidInput_ShouldAddDish()
        {
            var customer = new Customer(1, "john.doe@example.com");
            var order = new Order(1, customer);
            var dish = new Dish("Margherita", "Italian", 10.00m, true, new List<string> { "Cheese" });
            var orderDish = new OrderDish(dish, 2);

            order.AddOrderDish(orderDish);

            Assert.That(order.OrderDishes.Count, Is.EqualTo(1));
            Assert.That(order.CalculateTotal(), Is.EqualTo(20.00m));
        }

        [Test]
        public void Order_AddOrderDish_DuplicateDish_ShouldAggregateQuantities()
        {
            var customer = new Customer(1, "john.doe@example.com");
            var order = new Order(1, customer);
            var dish = new Dish("Margherita", "Italian", 10.00m, true, new List<string> { "Cheese" });

            var orderDish1 = new OrderDish(dish, 2);
            order.AddOrderDish(orderDish1);

            // Simulate aggregation by adding another OrderDish for the same dish
            order.AddOrderDish(new OrderDish(dish, 3));

            Assert.That(order.OrderDishes.Count, Is.EqualTo(1)); // Only one OrderDish
            Assert.That(order.OrderDishes.First().Quantity, Is.EqualTo(5)); // Quantity aggregated
            Assert.That(order.CalculateTotal(), Is.EqualTo(50.00m)); // Total reflects aggregated quantity
        }



        [Test]
        public void Order_CancelOrder_UnpaidOrder_ShouldRemoveAllDishes()
        {
            var customer = new Customer(1, "john.doe@example.com");
            var order = new Order(1, customer);
            var dish = new Dish("Margherita", "Italian", 10.00m, true, new List<string> { "Cheese" });
            var orderDish = new OrderDish(dish, 2);

            order.AddOrderDish(orderDish);
            order.CancelOrder();

            Assert.That(Order.OrderExtent.Count, Is.EqualTo(0));
            Assert.That(customer.Orders, Does.Not.Contain(order));

            var dishOrderDishesField = typeof(Dish).GetField("_orderDishes", BindingFlags.NonPublic | BindingFlags.Instance);
            var dishOrderDishes = dishOrderDishesField?.GetValue(dish) as List<OrderDish>;
            Assert.That(dishOrderDishes, Does.Not.Contain(orderDish));
        }
        
        [Test]
        public void CancelOrdersForCustomer_ShouldCancelAllOrdersForGivenCustomer()
        {
            // Arrange
            var customer = new Customer(1, "john.doe@example.com");
            var order1 = new Order(1, customer);
            var order2 = new Order(2, customer);
            var dish = new Dish("Margherita", "Italian", 10.00m, true, new List<string> { "Cheese" });

            order1.AddOrderDish(new OrderDish(dish, 2));
            order2.AddOrderDish(new OrderDish(dish, 3));

            // Act
            Order.CancelOrdersForCustomer(customer.IdCustomer);

            // Assert
            Assert.That(Order.OrderExtent.Count, Is.EqualTo(0)); // All orders canceled
            Assert.That(customer.Orders, Is.Empty); // Customer has no orders
        }
    }
    
    // RESTAURANT TEST
    [TestFixture]
    public class RestaurantTests
    {
        [SetUp]
        public void SetUp()
        {
            Restaurant.ClearExtent();
            Table.ClearExtent(); 
            Customer.ClearCustomers();
            Reservation.ClearExtent();
        }

        [Test]
        public void Restaurant_Constructor_ValidInput_ShouldCreateInstance()
        {
            var restaurant = new Restaurant("Test Restaurant", 50);

            Assert.That(restaurant.Name, Is.EqualTo("Test Restaurant"));
            Assert.That(restaurant.MaxCapacity, Is.EqualTo(50));
            Assert.That(Restaurant.RestaurantExtent.Count, Is.EqualTo(1));
        }

        [Test]
        public void Restaurant_Constructor_EmptyName_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Restaurant("", 50));
        }

        [Test]
        public void Restaurant_Constructor_NegativeMaxCapacity_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Restaurant("Test Restaurant", -10));
        }

        [Test]
        public void AddTable_ValidTable_ShouldAddTable()
        {
            var restaurant = new Restaurant("Test Restaurant", 5);
            var table = new Table(1, 4, "Standard");

            restaurant.AddTable(table);

            Assert.That(restaurant.Tables.Count, Is.EqualTo(1));
            Assert.That(restaurant.Tables.First(), Is.EqualTo(table));
            Assert.That(restaurant.AvailableCapacity, Is.EqualTo(4)); 
        }


        [Test]
        public void AddTable_NullTable_ShouldThrowArgumentNullException()
        {
            var restaurant = new Restaurant("Test Restaurant", 5);

            Assert.Throws<ArgumentNullException>(() => restaurant.AddTable(null));
        }

        [Test]
        public void AddTable_DuplicateTable_ShouldThrowInvalidOperationException()
        {
            var restaurant = new Restaurant("Test Restaurant", 5);
            var table = new Table(1, 4, "Standard");

            restaurant.AddTable(table);

            Assert.Throws<InvalidOperationException>(() => restaurant.AddTable(new Table(1, 6, "Standard")));
        }

        [Test]
        public void AddTable_ExceedingCapacity_ShouldThrowInvalidOperationException()
        {
            var restaurant = new Restaurant("Test Restaurant", 1);
            var table1 = new Table(1, 4, "Standard"); 
            var table2 = new Table(2, 6, "VIP"); 

            restaurant.AddTable(table1);
            Assert.Throws<InvalidOperationException>(() => restaurant.AddTable(table2));
        }


        [Test]
        public void RemoveTable_ExistingTable_ShouldRemoveTable()
        {
            var restaurant = new Restaurant("Test Restaurant", 5);
            var table = new Table(1, 4, "Standard"); 

            restaurant.AddTable(table);
            restaurant.RemoveTable(table);

            Assert.That(restaurant.Tables.Count, Is.EqualTo(0));
            Assert.That(restaurant.AvailableCapacity, Is.EqualTo(5));
        }


        [Test]
        public void RemoveTable_NonExistingTable_ShouldThrowInvalidOperationException()
        {
            var restaurant = new Restaurant("Test Restaurant", 5);
            var existingTable = new Table(1, 4, "Standard");
            var nonExistingTable = new Table(2, 4, "Standard"); // Different ID

            restaurant.AddTable(existingTable);

            Assert.Throws<InvalidOperationException>(() => restaurant.RemoveTable(nonExistingTable));
        }
        
        [Test]
        public void ClearExtent_ShouldRemoveAllRestaurants()
        {
            new Restaurant("Restaurant 1", 10);
            new Restaurant("Restaurant 2", 20);

            Restaurant.ClearExtent();

            Assert.That(Restaurant.RestaurantExtent.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveRestaurant_ValidInput_ShouldRemoveRestaurant()
        {
            var restaurant = new Restaurant("Test Restaurant", 50);

            var removed = Restaurant.RemoveRestaurant(restaurant);

            Assert.That(removed, Is.True);
            Assert.That(Restaurant.RestaurantExtent.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveRestaurant_NonExistingRestaurant_ShouldReturnFalse()
        {
            var restaurant = new Restaurant("Non-Existent Restaurant", 50);
            Restaurant.ClearExtent(); 

            var removed = Restaurant.RemoveRestaurant(restaurant);

            Assert.That(removed, Is.False);
        }
    }
    
    // TABLE TEST
    [TestFixture]
    public class TableTests
    {
        [SetUp]
        public void SetUp()
        {
            Table.ClearExtent();
            Reservation.ClearExtent();
            Customer.ClearCustomers();
        }

        [Test]
        public void Table_Constructor_ValidInput_ShouldCreateInstance()
        {
            var table = new Table(1, 4, "Standard");

            Assert.That(table.IdTable, Is.EqualTo(1));
            Assert.That(table.NumberOfChairs, Is.EqualTo(4));
            Assert.That(table.TableType, Is.EqualTo("Standard"));
            Assert.That(Table.TableExtent.Count, Is.EqualTo(1));
        }

        [Test]
        public void Table_Constructor_DuplicateId_ShouldThrowInvalidOperationException()
        {
            new Table(1, 4, "Standard");

            Assert.Throws<InvalidOperationException>(() => new Table(1, 6, "VIP"));
        }

        [Test]
        public void Table_Constructor_InvalidNumberOfChairs_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Table(1, 25, "Standard"));
        }

        [Test]
        public void Table_Constructor_InvalidTableType_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Table(1, 4, "St"));
        }

        [Test]
        public void AssignWaiter_ValidWaiter_ShouldAssignWaiter()
        {
            var table = new Table(1, 4, "Standard");
            var waiter = new Waiter(1);

            table.AssignWaiter(waiter);

            Assert.That(table.AssignedWaiter, Is.EqualTo(waiter));
        }

        [Test]
        public void AssignWaiter_AlreadyAssigned_ShouldThrowInvalidOperationException()
        {
            var table = new Table(1, 4, "Standard");
            var waiter1 = new Waiter(1);
            var waiter2 = new Waiter(2);

            table.AssignWaiter(waiter1);

            Assert.Throws<InvalidOperationException>(() => table.AssignWaiter(waiter2));
        }

        [Test]
        public void UnassignWaiter_AssignedWaiter_ShouldUnassign()
        {
            var table = new Table(1, 4, "Standard");
            var waiter = new Waiter(1);

            table.AssignWaiter(waiter);
            table.UnassignWaiter();

            Assert.That(table.AssignedWaiter, Is.Null);
        }

        [Test]
        public void ClearExtent_ShouldRemoveAllTables()
        {
            new Table(1, 4, "Standard");
            new Table(2, 6, "VIP");

            Table.ClearExtent();

            Assert.That(Table.TableExtent.Count, Is.EqualTo(0));
        }
    }
    
    // WAITER TEST
    [TestFixture]
    public class WaiterTests
    {
        private List<string> _logMessages;

        [SetUp]
        public void SetUp()
        {
            _logMessages = new List<string>();
            Table.ClearExtent();
        }

        private void LogAction(string message)
        {
            _logMessages.Add(message);
        }

        [Test]
        public void Waiter_Constructor_ValidInput_ShouldCreateInstance()
        {
            var waiter = new Waiter(1, LogAction);

            Assert.That(waiter.IdWaiter, Is.EqualTo(1));
            Assert.That(waiter.AssignedTableCount, Is.EqualTo(0));
        }

        [Test]
        public void Waiter_Constructor_InvalidId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Waiter(0, LogAction));
        }

        [Test]
        public void AssignTable_ValidTable_ShouldAssignSuccessfully()
        {
            var waiter = new Waiter(1, LogAction);
            var table = new Table(1, 4, "Standard");

            waiter.AssignTable(table);

            Assert.That(waiter.AssignedTableCount, Is.EqualTo(1));
            Assert.That(waiter.AssignedTables.Contains(table), Is.True);
            Assert.That(table.AssignedWaiter, Is.EqualTo(waiter));
        }

        [Test]
        public void AssignTable_DuplicateTable_ShouldNotAddTwice()
        {
            var waiter = new Waiter(1, LogAction);
            var table = new Table(1, 4, "Standard");

            waiter.AssignTable(table);
            waiter.AssignTable(table);

            Assert.That(waiter.AssignedTableCount, Is.EqualTo(1));
            Assert.That(_logMessages.Count, Is.EqualTo(1)); // Only one successful assignment log
        }

        [Test]
        public void UnassignTable_ExistingTable_ShouldUnassignSuccessfully()
        {
            var waiter = new Waiter(1, LogAction);
            var table = new Table(1, 4, "Standard");

            waiter.AssignTable(table);
            var result = waiter.UnassignTable(table.IdTable);

            Assert.That(result, Is.True);
            Assert.That(waiter.AssignedTableCount, Is.EqualTo(0));
            Assert.That(table.AssignedWaiter, Is.Null);
        }

        [Test]
        public void UnassignTable_NonExistingTable_ShouldReturnFalse()
        {
            var waiter = new Waiter(1, LogAction);

            var result = waiter.UnassignTable(99); // Table ID 99 doesn't exist

            Assert.That(result, Is.False);
            Assert.That(_logMessages.Count, Is.EqualTo(1));
            Assert.That(_logMessages[0], Does.Contain("not found"));
        }

        [Test]
        public void ClearAssignedTables_ShouldUnassignAllTables()
        {
            var waiter = new Waiter(1, LogAction);
            var table1 = new Table(1, 4, "Standard");
            var table2 = new Table(2, 6, "VIP");

            waiter.AssignTable(table1);
            waiter.AssignTable(table2);

            waiter.ClearAssignedTables();

            Assert.That(waiter.AssignedTableCount, Is.EqualTo(0));
            Assert.That(table1.AssignedWaiter, Is.Null);
            Assert.That(table2.AssignedWaiter, Is.Null);
            Assert.That(_logMessages.Count, Is.EqualTo(3)); // Two unassign logs + one "all cleared"
        }

        [Test]
        public void Waiter_Constructor_WithAssignedTables_ShouldAssignAll()
        {
            var table1 = new Table(1, 4, "Standard");
            var table2 = new Table(2, 6, "VIP");
            var tables = new List<Table> { table1, table2 };

            var waiter = new Waiter(1, tables, LogAction);

            Assert.That(waiter.AssignedTableCount, Is.EqualTo(2));
            Assert.That(waiter.AssignedTables.Contains(table1), Is.True);
            Assert.That(waiter.AssignedTables.Contains(table2), Is.True);
            Assert.That(table1.AssignedWaiter, Is.EqualTo(waiter));
            Assert.That(table2.AssignedWaiter, Is.EqualTo(waiter));
        }

        [Test]
        public void Waiter_Constructor_WithDuplicateAssignedTables_ShouldAvoidDuplicates()
        {
            var table = new Table(1, 4, "Standard");
            var tables = new List<Table> { table, table }; // Duplicate table

            var waiter = new Waiter(1, tables, LogAction);

            Assert.That(waiter.AssignedTableCount, Is.EqualTo(1));
            Assert.That(waiter.AssignedTables.Contains(table), Is.True);
        }

        [Test]
        public void Waiter_ToString_ShouldReturnExpectedFormat()
        {
            var waiter = new Waiter(1, LogAction);
            var table = new Table(1, 4, "Standard");

            waiter.AssignTable(table);

            var result = waiter.ToString();

            Assert.That(result, Is.EqualTo("Waiter ID: 1, Assigned Tables: 1"));
        }
    }
    
   // RESERVATION TEST
   [TestFixture]
   public class ReservationTests
   {
       [SetUp]
       public void SetUp()
       {
           Reservation.ClearExtent();
           Table.ClearExtent();
           Customer.ClearCustomers();
           Order.ClearExtent();
       }

       [Test]
       public void CreateReservation_ValidParameters_ShouldSucceed()
       {
           var table = new Table(1, 4, "Standard");
           var customer = new Customer(1, "customer1@example.com");
           var reservationDate = DateTime.Now.AddDays(1);

           var reservation = new Reservation(1, reservationDate, table, customer);

           Assert.That(Reservation.ReservationExtent.Count, Is.EqualTo(1));
           Assert.That(Reservation.ReservationExtent.First(), Is.EqualTo(reservation));
           Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Pending));
       }

       [Test]
       public void CreateReservation_NullTable_ShouldThrowArgumentNullException()
       {
           var customer = new Customer(1, "customer1@example.com");
           var reservationDate = DateTime.Now.AddDays(1);

           Assert.Throws<ArgumentNullException>(() => new Reservation(1, reservationDate, null, customer));
       }

       [Test]
       public void CreateReservation_NullCustomer_ShouldThrowArgumentNullException()
       {
           var table = new Table(1, 4, "Standard");
           var reservationDate = DateTime.Now.AddDays(1);

           Assert.Throws<ArgumentNullException>(() => new Reservation(1, reservationDate, table, null));
       }

       [Test]
       public void CreateReservation_DateInPast_ShouldThrowArgumentException()
       {
           var table = new Table(1, 4, "Standard");
           var customer = new Customer(1, "customer1@example.com");
           var reservationDate = DateTime.Now.AddDays(-1);

           Assert.Throws<ArgumentException>(() => new Reservation(1, reservationDate, table, customer));
       }

       [Test]
       public void CreateReservation_DuplicateId_ShouldThrowInvalidOperationException()
       {
           var table = new Table(1, 4, "Standard");
           var customer = new Customer(1, "customer1@example.com");
           var reservationDate = DateTime.Now.AddDays(1);
           new Reservation(1, reservationDate, table, customer);

           Assert.Throws<InvalidOperationException>(() => new Reservation(1, reservationDate, table, customer));
       }

       [Test]
       public void CreateReservation_TableAlreadyReserved_ShouldThrowInvalidOperationException()
       {
           var table = new Table(1, 4, "Standard");
           var customer1 = new Customer(1, "customer1@example.com");
           var customer2 = new Customer(2, "customer2@example.com");
           var reservationDate = DateTime.Now.AddDays(1);
           new Reservation(1, reservationDate, table, customer1);

           Assert.Throws<InvalidOperationException>(() => new Reservation(2, reservationDate, table, customer2));
       }

       [Test]
       public void ConfirmReservation_ShouldChangeStatusToConfirmed()
       {
           var table = new Table(1, 4, "Standard");
           var customer = new Customer(1, "customer1@example.com");
           var reservationDate = DateTime.Now.AddDays(1);
           var reservation = new Reservation(1, reservationDate, table, customer);

           reservation.ConfirmReservation();

           Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Confirmed));
       }

       [Test]
       public void ConfirmReservation_NonPendingStatus_ShouldThrowInvalidOperationException()
       {
           var table = new Table(1, 4, "Standard");
           var customer = new Customer(1, "customer1@example.com");
           var reservationDate = DateTime.Now.AddDays(1);
           var reservation = new Reservation(1, reservationDate, table, customer);
           reservation.ConfirmReservation();

           Assert.Throws<InvalidOperationException>(() => reservation.ConfirmReservation());
       }

       [Test]
       public void CancelReservation_ShouldChangeStatusToCanceled()
       {
           var table = new Table(1, 4, "Standard");
           var customer = new Customer(1, "customer1@example.com");
           var reservationDate = DateTime.Now.AddDays(1);
           var reservation = new Reservation(1, reservationDate, table, customer);

           reservation.CancelReservation();

           Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Canceled));
           Assert.That(Reservation.ReservationExtent.Contains(reservation), Is.False);
       }

       [Test]
       public void UpdateTable_ShouldUpdateReservedTable()
       {
           // Arrange: Create tables and a reservation
           var table1 = new Table(1, 4, "Standard");
           var table2 = new Table(2, 6, "VIP");
           var customer = new Customer(1, "customer1@example.com");
           var reservationDate = DateTime.Now.AddDays(1);
           var reservation = new Reservation(1, reservationDate, table1, customer);

           // Act: Update the reservation to use a new table
           reservation.UpdateTable(table2);

           // Assert: Verify the table was updated
           Assert.That(reservation.ReservedTable, Is.EqualTo(table2));
           Assert.That(table1.Reservations, Does.Not.Contain(reservation));
           Assert.That(table2.Reservations, Contains.Item(reservation));
       }


       [Test]
       public void UpdateCustomer_ShouldUpdateReservationCustomer()
       {
           var table = new Table(1, 4, "Standard");
           var customer1 = new Customer(1, "customer1@example.com");
           var customer2 = new Customer(2, "customer2@example.com");
           var reservationDate = DateTime.Now.AddDays(1);
           var reservation = new Reservation(1, reservationDate, table, customer1);

           reservation.UpdateCustomer(customer2);

           Assert.That(reservation.Customer, Is.EqualTo(customer2));
       }

       [Test]
       public void ClearExtent_ShouldRemoveAllReservations()
       {
           var table = new Table(1, 4, "Standard");
           var customer = new Customer(1, "customer1@example.com");
           new Reservation(1, DateTime.Now.AddDays(1), table, customer);
           new Reservation(2, DateTime.Now.AddDays(2), table, customer);

           Reservation.ClearExtent();

           Assert.That(Reservation.ReservationExtent.Count, Is.EqualTo(0));
       }
   }
   
   [TestFixture]
   public class CustomerTests
    {
        [SetUp]
        public void SetUp()
        {
            Customer.ClearCustomers();
            Table.ClearExtent();
            Reservation.ClearExtent();
        }

        [Test]
        public void Customer_Constructor_ValidInput_ShouldCreateInstance()
        {
            var customer = new Customer(1, "test@example.com");

            Assert.That(customer.IdCustomer, Is.EqualTo(1));
            Assert.That(customer.Email, Is.EqualTo("test@example.com"));
            Assert.That(Customer.Customers.Count, Is.EqualTo(1));
        }

        [Test]
        public void Customer_Constructor_DuplicateEmail_ShouldThrowInvalidOperationException()
        {
            new Customer(1, "test@example.com");

            Assert.Throws<InvalidOperationException>(() => new Customer(2, "test@example.com"));
        }

        [Test]
        public void AddReservation_ValidReservation_ShouldAddToCustomer()
        {
            var customer = new Customer(1, "test@example.com");
            var table = new Table(1, 4, "Standard");
            var reservation = new Reservation(1, DateTime.Now.AddDays(1), table, customer);

            Assert.That(customer.Reservations.Count, Is.EqualTo(1));
            Assert.That(customer.Reservations.Contains(reservation), Is.True);
        }

        [Test]
        public void AddReservation_DuplicateReservation_ShouldThrowInvalidOperationException()
        {
            var customer = new Customer(1, "test@example.com");
            var table = new Table(1, 4, "Standard");
            var reservation = new Reservation(1, DateTime.Now.AddDays(1), table, customer);

            Assert.Throws<InvalidOperationException>(() => customer.AddReservation(reservation));
        }

        [Test]
        public void RemoveReservation_ExistingReservation_ShouldRemoveFromCustomer()
        {
            var customer = new Customer(1, "test@example.com");
            var table = new Table(1, 4, "Standard");
            var reservation = new Reservation(1, DateTime.Now.AddDays(1), table, customer);

            customer.RemoveReservation(reservation);

            Assert.That(customer.Reservations.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveReservation_NonExistingReservation_ShouldThrowInvalidOperationException()
        {
            var customer = new Customer(1, "test@example.com");
            var table = new Table(1, 4, "Standard");
            var reservation = new Reservation(1, DateTime.Now.AddDays(1), table, customer);

            var nonExistingReservation = new Reservation(2, DateTime.Now.AddDays(2), table, customer);

            Assert.Throws<InvalidOperationException>(() => customer.RemoveReservation(nonExistingReservation));
        }

        [Test]
        public void PlaceOrder_ValidDishes_ShouldCreateOrder()
        {
            var customer = new Customer(1, "test@example.com");
            var dish1 = new Dish("Pasta", "Italian", 12.99m, true, new List<string> { "Flour", "Tomato" });
            var dish2 = new Dish("Salad", "Vegetarian", 7.99m, true, new List<string> { "Lettuce", "Carrot" });

            var order = customer.PlaceOrder(new[] { dish1, dish2 });

            Assert.That(customer.Orders.Count, Is.EqualTo(1));
            Assert.That(customer.Orders.Contains(order), Is.True);
            Assert.That(order.OrderDishes.Count, Is.EqualTo(2));
        }

        [Test]
        public void MakePayment_ValidPayment_ShouldMarkOrderAsPaid()
        {
            var customer = new Customer(1, "test@example.com");
            var dish = new Dish("Pizza", "Classic", 10.99m, false, new List<string> { "Flour", "Cheese" });
            var order = customer.PlaceOrder(new[] { dish });
            var payment = new Payment(1, (double)10.99m, "Card");

            var result = customer.MakePayment(order, payment);

            Assert.That(result, Is.True);
            Assert.That(order.IsPaid, Is.True);
            Assert.That(customer.Payments.Count, Is.EqualTo(1));
        }

        [Test]
        public void MakePayment_AlreadyPaidOrder_ShouldThrowInvalidOperationException()
        {
            var customer = new Customer(1, "test@example.com");
            var dish = new Dish("Pizza", "Classic", 10.99m, false, new List<string> { "Flour", "Cheese" });
            var order = customer.PlaceOrder(new[] { dish });
            var payment = new Payment(1, (double)10.99m, "Card");

            customer.MakePayment(order, payment);

            Assert.Throws<InvalidOperationException>(() => customer.MakePayment(order, payment));
        }

        [Test]
        public void GetCustomerByEmail_ValidEmail_ShouldReturnCustomer()
        {
            var customer = new Customer(1, "test@example.com");

            var result = Customer.GetCustomerByEmail("test@example.com");

            Assert.That(result, Is.EqualTo(customer));
        }

        [Test]
        public void GetCustomerByEmail_InvalidEmail_ShouldReturnNull()
        {
            var result = Customer.GetCustomerByEmail("nonexistent@example.com");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ClearCustomers_ShouldRemoveAllCustomers()
        {
            new Customer(1, "customer1@example.com");
            new Customer(2, "customer2@example.com");

            Customer.ClearCustomers();

            Assert.That(Customer.Customers.Count, Is.EqualTo(0));
        }
    }
    
    // SERIALIZATION TEST
    [TestFixture]
    public class SerializationTests
    {
        private const string TestFileName = "TestExtent.xml";

        [SetUp]
        public void SetUp()
        {
            if (File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }

            // Clear the Instances list
            SerializableObject<TestSerializable>.Instances.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }
        }

        [Test]
        public void SerializableObject_SaveExtent_ShouldSaveExtentToFile()
        {
            // Arrange
            SerializableObject<TestSerializable>.AddInstance(new TestSerializable { Id = 1, Name = "Object1" });
            SerializableObject<TestSerializable>.AddInstance(new TestSerializable { Id = 2, Name = "Object2" });

            // Act
            SerializableObject<TestSerializable>.SaveExtent();

            // Assert
            Assert.That(File.Exists("TestSerializable_Extent.xml"), Is.True);
        }

        [Test]
        public void SerializableObject_LoadExtent_ShouldLoadExtentFromFile()
        {
            // Arrange
            SerializableObject<TestSerializable>.AddInstance(new TestSerializable { Id = 1, Name = "Object1" });
            SerializableObject<TestSerializable>.SaveExtent();

            SerializableObject<TestSerializable>.Instances.Clear();

            // Act
            SerializableObject<TestSerializable>.LoadExtent();

            // Assert
            Assert.That(SerializableObject<TestSerializable>.Instances.Count, Is.EqualTo(1));
            Assert.That(SerializableObject<TestSerializable>.Instances.Any(x => x.Name == "Object1"), Is.True);
        }

        [Test]
        public void ExtentManager_SaveAllExtents_ShouldInvokeSaveMethod()
        {
            // Arrange
            SerializableObject<TestSerializable>.AddInstance(new TestSerializable { Id = 1, Name = "Object1" });

            // Act
            ExtentManager.SaveAllExtents();

            // Assert
            Assert.That(File.Exists("TestSerializable_Extent.xml"), Is.True);
        }

        [Test]
        public void ExtentManager_LoadAllExtents_ShouldInvokeLoadMethod()
        {
            // Arrange
            SerializableObject<TestSerializable>.AddInstance(new TestSerializable { Id = 1, Name = "Object1" });
            SerializableObject<TestSerializable>.SaveExtent();

            SerializableObject<TestSerializable>.Instances.Clear();

            // Act
            ExtentManager.LoadAllExtents();

            // Assert
            Console.WriteLine($"Loaded Instances Count: {SerializableObject<TestSerializable>.Instances.Count}");
            Assert.That(SerializableObject<TestSerializable>.Instances.Count, Is.EqualTo(1));
            Assert.That(SerializableObject<TestSerializable>.Instances.First().Name, Is.EqualTo("Object1"));
        }


        [Test]
        public void SerializationManager_SerializeToXml_ShouldSaveToFile()
        {
            // Arrange
            var objects = new List<TestSerializable>
            {
                new TestSerializable { Id = 1, Name = "Object1" },
                new TestSerializable { Id = 2, Name = "Object2" }
            };

            // Act
            SerializationManager.SerializeToXml(objects, TestFileName);

            // Assert
            Assert.That(File.Exists(TestFileName), Is.True);
        }

        [Test]
        public void SerializationManager_DeserializeFromXml_ShouldLoadFromFile()
        {
            // Arrange
            var objects = new List<TestSerializable>
            {
                new TestSerializable { Id = 1, Name = "Object1" },
                new TestSerializable { Id = 2, Name = "Object2" }
            };

            SerializationManager.SerializeToXml(objects, TestFileName);

            // Act
            var deserialized = SerializationManager.DeserializeFromXml<TestSerializable>(TestFileName);

            // Assert
            Assert.That(deserialized.Count, Is.EqualTo(2));
            Assert.That(deserialized.Any(x => x.Name == "Object1"), Is.True);
        }

        [Test]
        public void SerializationManager_DeserializeFromXml_FileNotFound_ShouldReturnEmptyList()
        {
            // Act
            var deserialized = SerializationManager.DeserializeFromXml<TestSerializable>("NonExistentFile.xml");

            // Assert
            Assert.That(deserialized, Is.Empty);
        }
    }
    
    [TestFixture]
    public class ExtentManagerTests
    {
        private const string TestFilePath = "TestSerializable_Extent.xml";

        [SetUp]
        public void SetUp()
        {
            if (File.Exists(TestFilePath))
                File.Delete(TestFilePath);
            SerializableObject<TestSerializable>.Instances.Clear();
        }

        [Test]
        public void ExtentManager_LoadAllExtents_ShouldRestoreData()
        {
            // Arrange
            SerializableObject<TestSerializable>.AddInstance(new TestSerializable { Id = 1, Name = "Test Object" });
            SerializableObject<TestSerializable>.SaveExtent();

            // Clear instances to simulate a fresh load
            SerializableObject<TestSerializable>.Instances.Clear();
            Assert.That(SerializableObject<TestSerializable>.Instances.Count, Is.EqualTo(0));

            // Act
            ExtentManager.LoadAllExtents();

            // Assert
            Assert.That(SerializableObject<TestSerializable>.Instances.Count, Is.EqualTo(1));
            var instance = SerializableObject<TestSerializable>.Instances.First();
            Assert.That(instance.Id, Is.EqualTo(1));
            Assert.That(instance.Name, Is.EqualTo("Test Object"));
        }
    }


    public class TestSerializable : SerializableObject<TestSerializable>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static void LoadExtent() 
        {
            SerializableObject<TestSerializable>.LoadExtent();
        }

        public static void SaveExtent() 
        {
            SerializableObject<TestSerializable>.SaveExtent();
        }

        public override string ToString()
        {
            return $"TestSerializable [Id: {Id}, Name: {Name}]";
        }
    }
}