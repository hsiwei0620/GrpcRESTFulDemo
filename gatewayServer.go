package main

import (
	// "flag"
	"fmt"
	"net/http"
	"sync"

	"github.com/grpc-ecosystem/grpc-gateway/runtime"
	"golang.org/x/net/context"
	"google.golang.org/grpc"
	"path/to/project/protos/callAPIService"
) 

//Rungateway run start
func Rungateway(wg *sync.WaitGroup) error {

	defer wg.Done()
	fmt.Printf("running gateway server\n")
	ctx := context.Background()
	ctx, cancel := context.WithCancel(ctx)
	defer cancel()

	mux := runtime.NewServeMux()
	opts := []grpc.DialOption{grpc.WithInsecure()}
	err := callAPIService.RegisterAPI_ServiceHandlerFromEndpoint(ctx, mux, fmt.Sprintf(":%d", *gRPCport), opts)
	if err != nil {
		return err
	}

	return http.ListenAndServe(":9090", mux)
}
