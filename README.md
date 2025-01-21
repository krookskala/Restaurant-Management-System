
# Restaurant Management System


The **Restaurant Management System** is an interactive and extensible console application designed to streamline operations in the restaurant industry. It provides a robust platform for managing chefs, menus, orders, payments, reservations, and employees. Whether you're managing customer orders, tracking reservations, or assigning roles to employees, this system ensures efficient operations with a clean and user-friendly architecture.


![Image](https://github.com/user-attachments/assets/f1e01233-064b-4aca-b2dd-3ed6897c92b6)


## Table Of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Getting Started](#getting-started)
- [Detailed File Descriptions](#usage)
- [Contributing](#contributing)
- [Links](#links)
- [License](#license)
## Project Overview

The **Restaurant Management System** combines object-oriented programming principles with an event-driven architecture to provide a seamless restaurant management solution. From managing chefs and their special dishes to tracking customer orders and payments, the system is designed for scalability, performance, and extensibility.
## Features

**Core Features**

**1. Chef Management**
- **Role-Based Specialization:** Supports a hierarchy of chef roles, including:
    - **Executive Chef:** Oversees kitchen operations, prepares signature dishes, and trains sous chefs.
    - **Sous Chef:** Manages kitchen sections and assists in executing daily kitchen tasks.
    - **Line Chef:** Prepares specialized dishes based on their area of expertise.



- **Dish Tracking:** Tracks dishes prepared by each chef, including their specialties.


- **Restaurant Associations:** Maintains relationships between chefs and multiple restaurants, allowing dynamic allocation of resources.


- **Action Logging:** Tracks chef activities like dish preparation, restaurant assignments, and event-driven updates for review.

**2. Menu Management**
- **Dynamic Menu Creation:** Offers the flexibility to create and manage menus tailored to specific restaurant themes, such as daily specials or seasonal menus.


- **Multilingual Support:** CProvides menu translations to cater to diverse customer bases.


- **Cuisine Categorization:** Organize dishes by cuisine type (e.g., Italian, Indian, Chinese).


- **Dietary Filters:** Highlights vegetarian, vegan, and allergen-free options.


- **Qualified Dishes:** Allows tagging of specific dishes (e.g., "Chef's Special") to enhance their visibility.

**3. Customer and Membership Management**
- **Detailed Customer Profiles:** Maintains comprehensive customer records, including reservations, orders, and payment history.


- **Membership Rewards System:**
    - Allocates credit points for purchases.
    - Supports upgrades from non-member to member status, offering exclusive perks.


- **Reservation Management:**
    - Prevents booking conflicts with real-time availability checks.
    - Tracks statuses such as **Pending**, **Confirmed**, or **Canceled**.


- **Lifecycle Management:** Logs customer interactions, including order placements, reservation changes, and payment updates.

**4. Order and Payment Processing**
- **Comprehensive Order Lifecycle:**
    - Create, modify, and cancel orders dynamically.
    - Provide detailed order breakdowns, including individual dish details, quantities, and pricing.
  

- **Flexible Payment Integration:**
    - Support for multiple payment methods, including **Card** and **Cash**.
    - Secure processing with event-driven updates for **Completed** and **Refunded** statuses.


- **Real-Time Notifications:**  Ensures staff and customers are informed about payment and order status updates instantly.

**5. Reservation Management**
- **Double-Booking Prevention:** Implements strict table availability checks.


- **Reservation Lifecycle:** Manages reservation statuses with seamless transitions between **Pending**, **Confirmed** and **Canceled** states.


- **Integrated Logging:** Log all reservation events for accountability and review.


- **Customer and Table Association:** Establishes strong links between customer profiles and assigned tables for easier management.

**6. Employee Roles**
- **Role Diversity:**
    - **Waiters:** Manage assigned tables, assist customers, and coordinate with the kitchen staff.
    - **Valets:** Handle vehicle parking and ensure seamless customer arrivals.
    - **Trainees:** Track progress during training programs with defined durations and milestones.
    - **Managers:** Oversee employee assignments, reservations, and restaurant operations.


- **Structured Supervision:** Establishes clear reverse associations between managers and their supervised employees, ensuring accountability.


- **Real-Time Alerts:** Notify stakeholders of key events, such as task completions, new assignments, and employee milestones.

**Advanced Functionalities**


- **Event-Driven Notifications:** Real-time alerts for key operations, including training completions, payment processing, and reservation changes.


- **Enhanced Validation:**
    - Leverages data annotations to maintain consistent data integrity across entities
    - Includes custom validation mechanisms, such as the NonDefaultDateAttribute for proper date initialization.
  

- **Serialization and Persistence:**
    - Efficiently save and load system data using the **SerializationManager** and **SerializableObject**.
    - Maintain object state across sessions with ease.
  

- **Object Extent Management:** 
  - Centralized tracking of object instances with ExtentManager, ensuring system-wide consistency.
  

- **Extensibility:**
    - Easily add new roles, features, or functionalities without overhauling existing code.

    - Modular design allows the system to adapt to various restaurant environments.

## Getting Started

Follow these steps to set up and run the Restaurant Management System on your local machine.

### Prerequisites

Before starting, ensure your development environment includes the following:

- **.NET SDK:** Download and install the latest version from the official .NET website.
- **Git:** Required to clone the repository. Download from Git's official site.
- A suitable **IDE** (e.g., Visual Studio, JetBrains Rider, or Visual Studio Code).


### Installation

1. **Clone the repository**
   Open your terminal or command prompt and run the following command to clone the repository:

   ```bash
   git clone https://github.com/krookskala/Restaurant-Management-System
   cd ConsoleApp1
   ```
2. **Open the Project**
Launch your preferred IDE and open the solution file 

   ```bash
    ConsoleApp1.sln
   ```
3. **Restore Dependencies**
   Restore NuGet packages to ensure all dependencies are installed

   ```bash
    dotnet restore  
   ```

4. **Build the Project**
Compile the source code:

   ```bash
    dotnet build  
   ```

5. **Run the Application**
   Execute the application using the following command:

   ```bash
    dotnet run  
   ```    
6. **Interact with the System**
Once running, use the console interface to interact with the Restaurant Management System.

   
### Docker Deployment
From the root directory of the project, build a Docker image using the following command:

1. **Build the Docker Image**
   ```bash
   docker build -t restaurant-management-system .    
   ```

2. **Run the Docker Container**
Start a Docker container and expose the required ports:
    ```bash
   docker run -p 8080:8080 restaurant-management-system    
   ```

3. **Access the Application**

Navigate to **http://localhost:8080** in your browser or console interface to interact with the system.


## Detailed File Descriptions

- **/model:** The core of the system, encompassing all entities and their relationships, implementing the fundamental business logic:

    - **Chef.cs:** Manages chefs and their specialized roles, such as Executive Chef, Sous Chef, and Line Chef. Tracks prepared dishes, logs actions, and establishes relationships with restaurants.

    - **Customer.cs:** Handles customer profiles, including reservations, orders, payments, and membership statuses. Provides tools for customer lifecycle management and membership rewards.

    - **Dish.cs:** Represents dishes with attributes like name, cuisine type, price, and ingredients. Supports tracking dietary preferences (e.g., vegetarian) and associations with menus and orders.

    - **Employee.cs:** Abstract base class for managing employee information, including work details, department assignments, and supervision hierarchies.

    - **Experienced.cs:** Tracks experienced mentors and their activities, including training and mentoring trainees. Facilitates event-driven notifications for mentorship actions.

    - **Manager.cs:** Oversees operations such as assigning employees to roles, supervising tasks, and managing table assignments. Tracks relationships with supervised employees.

    - **Member.cs:** Tracks customer membership details, including reward systems like credit points. Supports upgrading non-members to members and handling membership benefits.

    - **Menu.cs:** Manages dynamic menus with multilingual support. Organizes dishes by cuisine type, dietary preferences, and qualifiers like "Chef's Special."

    - **Order.cs:** Handles the complete lifecycle of customer orders, linking them to specific dishes, customers, and payment statuses. Provides detailed order breakdowns.

    - **OrderDish.cs:** Links orders to individual dishes and tracks quantities, unit prices, and total costs. Ensures reverse associations with the Dish class.

    - **Payment.cs:** Manages payments, including support for cash and card transactions. Tracks payment statuses (e.g., Pending, Completed, Refunded) and facilitates refunds with logging.

    - **Person.cs:** Base class for entities such as customers and employees, encapsulating common attributes like ID, name, birth date, and phone number.

    - **Reservation.cs:** Tracks reservations for tables, including statuses (Pending, Confirmed, Canceled). Prevents double-booking and maintains associations with customers and tables.

    - **Restaurant.cs:** Represents restaurants with attributes like name and capacity. Manages tables, reservations, and restaurant-level operations. Tracks available and reserved capacity.

    - **Specialist.cs:** Represents experts specializing in fields like recipe design. Supports designing new recipes and managing expert-level activities within the restaurant system.

    - **Table.cs:** Tracks table attributes like ID, type, and number of chairs. Manages reservations, waiter assignments, and associated capacity.

    - **Trainee.cs:** Represents employees in training. Tracks training durations, milestones, and completion statuses, with automatic end-date calculations.

    - **Valet.cs:** Represents valets responsible for parking cars and managing assigned locations. Tracks parking activities and logs actions using event-driven notifications.

    - **Waiter.cs:** Tracks waiter operations, including table assignments and coordination with restaurant staff. Supports dynamic reassignment and logging of table-related activities.

- **/services:** Handles auxiliary operations, system-wide functionality, and data persistence:

    - **ExtentManager.cs:** Centralized manager for tracking object instances (extents) across the system. Facilitates querying and persistence of system entities.

    - **SerializableObject.cs:** Base class for all serializable objects in the system. Provides utility methods for serialization and deserialization of data.

    - **SerializationManager.cs:** Handles the saving and loading of data to/from files using serialization. Supports file-based persistence for system entities and logs.

- **/validation:** Contains custom validation logic to enforce data integrity across the system:

    - **NonDefaultDateAttribute.cs:** Custom validation attribute for ensuring dates are properly initialized and not left at their default value. Used extensively for validating fields like reservation dates and employee hiring dates.

- **/test:** Ensures the reliability and robustness of the system through automated testing:

    - **UnitTest1.cs:** Contains unit tests for critical system components. Ensures functional accuracy and reliability for customer management, orders, payments, and more. Utilizes NUnit for a comprehensive testing framework.
## Contributing

Contributions are welcome!

If you find any issues or have ideas for improvements, feel free to open an issue or submit a pull request.

Please make sure to follow the project's code of conduct.

1. **Fork the repository**
2. **Create your feature branch (git checkout -b feature/YourFeature)**
3. **Commit your changes (git commit -am 'Add some feature')**
4. **Push to the branch (git push origin feature/YourFeature)**
5. **Open a pull request**



## Links

[![Gmail](https://img.shields.io/badge/ismailsariarslan7@gmail.com-D14836?style=for-the-badge&logo=gmail&logoColor=white)](ismailsariarslan7@gmail.com)

[![instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/ismailsariarslan/)

[![linkedin](https://img.shields.io/badge/linkedin-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/ismailsariarslan/)
## License

The code in this repository is licensed under the [MIT License.](https://choosealicense.com/licenses/mit/)
