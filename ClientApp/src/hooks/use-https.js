import { useState, useCallback } from 'react';
const useHttps = () => 
{
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
  
    const sendRequestToFetch = useCallback( async (requestConfig, applyDataFnc) => 
    {
      setIsLoading(true);
      setError(null);

      try 
      {
        const response = await fetch(
          requestConfig.url, {
            method: requestConfig.method ? requestConfig.method : 'GET',
            headers: requestConfig.headers ? requestConfig.headers : {},
            credentials:"include",
            body: requestConfig.body ? JSON.stringify(requestConfig.body) : null
          });
        
          if (!response.ok) 
          {
            throw new Error('Request failed!');
          }
          const data = await response.json();
          applyDataFnc(data);
      }
      catch(err)
      {
        setError(err.message || 'Something went wrong!');
      }
      setIsLoading(false);
    }, []);

    return({
      isLoading,
      error,
      sendRequestToFetch
     });
};

export default useHttps;