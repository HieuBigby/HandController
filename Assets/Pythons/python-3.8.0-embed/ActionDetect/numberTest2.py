import sys

def is_number(input_string):
    try:
        float(input_string)  # Try to convert the input to a floating-point number
        return True
    except ValueError:
        return False

def main():
    if len(sys.argv) != 2:
        print("Usage: python script.py input_string")
        return

    input_string = sys.argv[1]

    if is_number(input_string):
        print(f"'{input_string}' is a number.")
    else:
        print(f"'{input_string}' is not a number.")

if __name__ == "__main__":
    main()

