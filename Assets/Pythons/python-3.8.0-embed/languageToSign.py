from bidict import bidict
from underthesea import pos_tag
import numpy as np
import sys

def language_to_sign(text, replacements, vocab_dict):
    chunks = pos_tag(text)
    reverse_replacements = {value: key for key, value in replacements.items()}
    output_array = []

    i = 0
    while i < len(chunks):
        combined_word = chunks[i][0]
        # print('Checking: ' + combined_word)   
        found_replace = False
        temp_vocab = []

        for j in range(i + 1, len(chunks)):
            combined_word += " " + chunks[j][0]

            if combined_word in reverse_replacements:
                replace_sentence = reverse_replacements[combined_word]
                replace_words = replace_sentence.split()
                for word in replace_words:
                    if word in vocab_dict:
                        output_array.append(word)
                i = j + 1
                found_replace = True
                break
            if combined_word in vocab_dict:
                # print(combined_word + ' in dict!')
                temp_vocab.append(combined_word)
                i = j

        if not found_replace:
            if temp_vocab:
                output_array.extend(temp_vocab)
            else:
                word = chunks[i][0]
                word_type = chunks[i][1]
                # print(word + ' - ' + word_type)
                if word in vocab_dict:
                  output_array.append(word)
                if word_type == 'Np':
                  for character in word:
                    char_format = character.upper()
                    if char_format in reverse_replacements:
                      output_array.append(reverse_replacements[char_format])
                    else:
                      output_array.append(char_format)
            i += 1

    return output_array


def test():
    replacements = {
        "Tôi Tên": "Tôi tên là",
        "Vui Gặp": "Rất vui được gặp bạn",
        "E^/" : "Ế"
    }   
    vocab_dict = ['Xin chào', 'Tôi', 'Tên', 'H', 'I', 'Ế', 'U', 'Em']
    text = sys.argv[1]
    result = language_to_sign(text, replacements, vocab_dict)
    print(result)

if __name__ == '__main__':
    test()