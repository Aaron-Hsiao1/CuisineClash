def add_book(inventory):
    title = input("Enter the book title: ")
    amount = int(input("Enter the number of copies to add: "))
    if (title in inventory):
        inventory[title] += amount
        print("\n" + str(amount) + " more copies of " + str(title) + " have been added.")
    else:
        inventory[title] = amount
        print("\n" + str(amount) + " copies of " + str(title) + " have been added.")
        
    
def find_book(inventory):
    title = input("Enter the book title to find: ")
    if (title in inventory):
        print(f"\nThere are " + str(inventory[title]) + " copies of '" + str(title) + "' in stock.")
    else:
        print(f"\nNo copies of '" + str(title) + "' are in stock.")

def remove_book(inventory):
    title = input("Enter the book title to remove: ")
    if (title in inventory):
        if (inventory[title] > 0):
            inventory[title] -= 1
            print("\nRemoved one copy of " + str(title))
            if (inventory[title] == 0):
                keep = input("\nNo more copies left. Would you like to keep the book in the system with 0 copies? (yes/no): ")
                if (keep == ("no") or keep == ("No") or keep == ("NO")):
                    del inventory[title]
        else:
            print("Removal cannot happen, there are 0 copies in stock.")
    else:
        print(f"'" + str(title) + "' is not in the inventory.")

inventory = {}
while True:
    print("\nWelcome to the Library Inventory System")
    print("1. to Add a book")
    print("2. to Remove a book")
    print("3. to Find a book")
    print("4. to Quit")
        
    choice = input("\nEnter your choice: ")
        
    if (choice == ("1")):
        add_book(inventory)
    elif (choice == ("2")):
        remove_book(inventory)
    elif (choice == ("3")):
        find_book(inventory)
    elif (choice == ("4")):
        break
    else:
        print("Invalid choice")
            
print("Exit")

