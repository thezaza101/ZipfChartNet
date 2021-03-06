require("tidyverse")
require("jsonlite")

initial.options <- commandArgs(trailingOnly = FALSE)
file.arg.name <- "--file="
script.name <- sub(file.arg.name, "", initial.options[grep(file.arg.name, initial.options)])
script.dirname <- dirname(script.name)
print(script.dirname)
setwd(script.dirname)

settings <- read_json("settings.json")

lines <- read_lines("data.txt")

if(settings$removePunctuation){
  # Remove the punctuation from the book
  lines <- gsub('[[:punct:]]+','',lines)
}

# Split the data by spaces
words <- strsplit(lines," ",fixed = T)

# Remove "character(0)" type
words <- words[!sapply(words, identical, character(0))]

# Flatten the list
# words <- flatten(words)

if(!settings$caseSensitive){
  # Covert all the words to lowercase
  words <- lapply(words, tolower)
}

# Convert the list to a dataframe
words_df = data.frame(unlist(words))

# Count the number of words
count <- words_df %>% 
  group_by(unlist.words.) %>% 
  filter(!unlist.words.=="") %>% 
  count(sort=TRUE) %>%
  arrange(desc(n)) %>%
  head(settings$numCount)

# Determine the maximum value of n
maxval = head(count,1)$n

# Add row number as a column
count <- tibble::rowid_to_column(count, "row_number")

# Add Zipf value based on the maximum value of n
count <- count %>%
  arrange(desc(n)) %>%
  mutate(zipfval = maxval/row_number)

# Plot the findings
p <- ggplot(count, aes(x=reorder(unlist.words., -n), y=n)) +
  geom_bar(stat = "identity", fill="deepskyblue1") + 
  geom_bar(aes(y=zipfval),stat = "identity", fill="dodgerblue4") + 
  geom_line(aes(x=row_number, y=n, color="Actual frequency"), size=1) + 
  geom_line(aes(x=row_number, y=zipfval, color="Zipf frequency"), size=1) +
  coord_cartesian(xlim = c(0, 50), ylim = c(0, maxval)) +
  labs(title=settings$chartTitle,
       subtitle=settings$chartSubTitle,
       x="Word",
       y="Frequency",
       colour="") +
  theme(axis.text.x = element_text(angle = 90))

ggsave(str_c(settings$id,".png"), plot = p, device = "png")