.PHONY: all clean install uninstall headers

SRC = src
OBJ = obj
BIN = bin

EXE = $(BIN)/fontpp
DST = ../..

HDRFILES = $(wildcard $(SRC)/*.h)
SRCFILES = $(filter %.c %.cpp, $(wildcard $(SRC)/*))
OBJFILES = $(patsubst $(SRC)/%,$(OBJ)/%.o,$(basename $(SRCFILES)))

WARNINGS = -Wall -Wextra -Wpedantic
BUILDOBJ = gcc $(WARNINGS) -c -g -o $@ $<
BUILDEXE = g++ $(WARNINGS) -o $@ $^ -lwoff2enc

VPATH = $(SRC)

all: $(EXE)

clean:
	rm -f $(OBJ)/*.o
	rm -f $(EXE)

install: all
	mkdir -pv $(DST)/$(dir $(EXE))
	cp $(EXE) $(DST)/$(EXE)

uninstall:
	rm -f $(DST)/$(EXE)

headers:
	gcc $(WARNINGS) -o /dev/null $(HDRFILES)

$(OBJFILES): | $(OBJ)

$(OBJ):
	mkdir $(OBJ)

$(BIN):
	mkdir $(BIN)

$(OBJ)/inout.o: inout.c inout.h types.h
	$(BUILDOBJ)

$(OBJ)/woff2.o: woff2.cpp types.h woff2.h
	$(BUILDOBJ)

$(OBJ)/main.o: main.c inout.h types.h woff2.h
	$(BUILDOBJ)

$(EXE): $(OBJFILES) | $(BIN)
	$(BUILDEXE)
