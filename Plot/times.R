library(dplyr)
library(ggplot2)

dat <- read.csv2("C:\\Users\\zuzia\\OneDrive\\Dokumenty\\Magisterka\\Chromki\\InterCol\\Plot\\times.csv", sep = ",")

func <- function(alg){
  datF <- dat %>% filter(Algorithm == alg)
  
  ggplot(datF, aes(x = Vertices.Count, y = Time, color = Edges.Percent)) +
    geom_line() +
    labs(x = "Iloœæ wierzcho³ków w grafie", y = "Czas obliczeñ [ms]", color = "Gêstoœæ grafu") +
    facet_wrap(vars(Calls.Limit))
}

func("Matching")
func("Edge")
