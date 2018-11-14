package APISource

import "strconv"

// time sub
func timeSub(t1, t2 string) int {

	time1, _ := strconv.Atoi(t1)
	time2, _ := strconv.Atoi(t2)

	return (time1 - time2)
}

var (
	foundFlag bool //load_recipe_drug , move_in_finish_drug
	gittest   string
	testGut   string
)
