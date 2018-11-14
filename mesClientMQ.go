package main

import (
	"flag"
	"sync" // API "path/to/project/APISource"

	"github.com/golang/glog"
)

var (
	gRPCport    = flag.Uint("grpc_port", 8080, "port for gRPC")
	gatewayPort = flag.Uint("gateway_port", 9090, "port for RESTful API")
	mesServer   = flag.String("mes", "192.1.1.103", "mes server")
)

func main() {

	wg := sync.WaitGroup{}
	defer wg.Wait()

	wg.Add(1)

	flag.Parse()
	defer glog.Flush()

	go func() {
		if err := Rungateway(&wg); err != nil {
			glog.Fatal(err)
		}
	}()

	wg.Add(1)

	go func() {
		if err := RungRPC(&wg); err != nil {
			glog.Fatal(err)
		}
	}()

}
